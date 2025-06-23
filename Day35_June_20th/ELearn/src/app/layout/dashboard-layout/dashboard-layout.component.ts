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

  role = signal(this.authService.getRole()?.toLowerCase());

  searchControl = new FormControl('');
  suggestions = signal<{ id: string; title: string }[]>([]);

  showProfileMenu = signal(false); // dropdown toggle

  constructor() {
    if (this.role() === 'student') {
      this.setupSearch();
    }
  }

  logout() {
    this.authService.logout();
  }

  toggleProfileMenu() {
    this.showProfileMenu.update(val => !val);
  }

  navigateToProfile() {
    this.router.navigate(['/profile']);
    this.showProfileMenu.set(false);
  }

  navigateToEditProfile() {
    this.router.navigate(['/profile/edit']);
    this.showProfileMenu.set(false);
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

  menuItems = computed(() => {
    switch (this.role()) {
      case 'admin':
        return [
          { label: 'Dashboard', path: '/admin-dashboard' },
          { label: 'Manage Users', path: '/admin-dashboard/users' },
          { label: 'Reports', path: '/admin-dashboard/reports' },
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
        return [];
    }
  });
}
