import {
  Component,
  computed,
  inject,
  signal
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../features/auth/auth.service';
import { StudentService } from '../../features/student/services/student.service';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged, switchMap, tap } from 'rxjs';
import { NotificationService } from '../../shared/services/notification.service';

@Component({
  selector: 'app-dashboard-layout',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule],
  templateUrl: './dashboard-layout.component.html',
})
export class DashboardLayoutComponent {
  private authService = inject(AuthService);
  private router = inject(Router);
  private studentService = inject(StudentService);
  private notificationService = inject(NotificationService);

  role = signal(this.authService.getRole()?.toLowerCase());

  searchControl = new FormControl('');
  suggestions = signal<{ id: string; title: string }[]>([]);
  showProfileMenu = signal(false);
  showNotifications = signal(false);
  notifications = signal<{ message: string; timestamp: Date }[]>([]);

  constructor() {
  
    const stored = localStorage.getItem('notifications');
    if (stored) {
      try {
        const parsed = JSON.parse(stored) as { message: string; timestamp: string }[];
        this.notifications.set(
          parsed.map(n => ({ ...n, timestamp: new Date(n.timestamp) }))
        );
      } catch {
        localStorage.removeItem('notifications');
      }
    }

    if (this.role() === 'student') {
      this.setupSearch();
    }

    this.initNotificationConnection();
  }

  logout() {
    this.authService.logout();
    localStorage.removeItem('notifications');
    this.notifications.set([]);
  }

  toggleProfileMenu() {
    this.showProfileMenu.update(val => !val);
  }

  navigateToProfile() {
    const role = this.role();
    const route =
      role === 'student'
        ? 'student-dashboard/profile'
        : role === 'instructor'
        ? 'instructor-dashboard/profile'
        : '/';
    this.router.navigate([route]);
    this.showProfileMenu.set(false);
  }

  toggleNotifications() {
    this.showNotifications.update(val => !val);
  }

  setupSearch() {
    this.searchControl.valueChanges
      .pipe(
        debounceTime(300),
        distinctUntilChanged(),
        switchMap(query => this.studentService.searchCourses(query ?? '')),
        tap(results => this.suggestions.set(results.slice(0, 5)))
      )
      .subscribe();
  }

  selectCourse(id: string) {
    this.router.navigate(['student-dashboard/course-detail', id]);
    this.suggestions.set([]);
    this.searchControl.setValue('');
  }

  private addNotification(message: string) {
    const newNotifications = [
      { message, timestamp: new Date() },
      ...this.notifications(),
    ].slice(0, 10);

    this.notifications.set(newNotifications);
    localStorage.setItem('notifications', JSON.stringify(newNotifications));
  }

initNotificationConnection() {
  const token = this.authService.getToken();
  const userRole = this.role();

  if (!token) {
    console.error('No auth token found. Cannot start notification connection.');
    return;
  }

  this.notificationService.startConnection(token);

  // Everyone can receive content upload notifications
  this.notificationService.contentNotification$.subscribe(notification => {
    if (notification) {
      const msg = `"${notification.fileName}" uploaded in "${notification.courseTitle}"`;
      this.addNotification(msg);
    }
  });

  // ✅ Only instructors should get enrollment notifications
  if (userRole === 'instructor') {
    this.notificationService.enrollmentNotification$.subscribe(notification => {
      if (notification) {
        const msg = `${notification.studentName} enrolled in "${notification.courseTitle}"`;
        this.addNotification(msg);
      }
    });
  }
}


  menuItems = computed(() => {
    switch (this.role()) {
      case 'admin':
        return [
          { label: 'Dashboard', path: '/admin-dashboard' },
          { label: 'Manage Users', path: '/admin-dashboard/users' },
          { label: 'Manage Courses', path: '/admin-dashboard/courses' },
        ];
      case 'instructor':
        return [
          { label: 'Dashboard', path: '/instructor-dashboard' },
          { label: 'Create Course', path: '/instructor-dashboard/create-course' },
          { label: 'My Courses', path: '/instructor-dashboard/my-courses' },
        ];
      case 'student':
        return [
          { label: 'Dashboard', path: '/student-dashboard' },
          { label: 'Enrolled Courses', path: '/student-dashboard/enrolled' },
          { label: 'Browse Courses', path: '/student-dashboard/browse' },
        ];
      default:
        return [
            
        ];
    }
  });
}
