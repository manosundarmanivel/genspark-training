import { Component, OnInit } from '@angular/core';
import { AdminService } from '../services/admin.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BehaviorSubject, catchError, combineLatest, of } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-manage-course',
  standalone: true,
  templateUrl: './manage-course.component.html',
  imports: [CommonModule, FormsModule, RouterModule],
})
export class ManageCourseComponent implements OnInit {
  private coursesSubject = new BehaviorSubject<{ data: any[] }>({ data: [] });
  private usersSubject = new BehaviorSubject<{ data: any[] }>({ data: [] });

  courses$ = this.coursesSubject.asObservable();
  users$ = this.usersSubject.asObservable();

  searchTerm = '';
  selectedStatus = '';
private selectedStatus$ = new BehaviorSubject<string>('');
  private searchTerm$ = new BehaviorSubject<string>('');

filteredCourses$ = combineLatest([
  this.courses$,
  this.searchTerm$.pipe(startWith('')),
  this.selectedStatus$.pipe(startWith(''))
]).pipe(
  map(([courseWrapper, term, status]) => {
    const courses = courseWrapper.data || [];
    const lowerTerm = term.toLowerCase().trim();
    return {
      data: courses.filter(c =>
        (!lowerTerm || c.title?.toLowerCase().includes(lowerTerm)) &&
        (!status ||
          (status === 'enabled' && !c.isDeleted) ||
          (status === 'disabled' && c.isDeleted))
      )
    };
  })
);

// Method for status filter change
onStatusFilterChange(status: string): void {
  this.selectedStatus$.next(status);
}

  constructor(private adminService: AdminService) {}

  ngOnInit(): void {
    this.reloadData();
  }

  reloadData(): void {
    this.adminService.getAllCourses()
      .pipe(catchError(() => of({ data: [] })))
      .subscribe(c => this.coursesSubject.next(c));

    this.adminService.getAllUsers()
      .pipe(catchError(() => of({ data: [] })))
      .subscribe(u => this.usersSubject.next(u));
  }

  onSearchChange(term: string): void {
    this.searchTerm$.next(term);
  }

  toggleCourseStatus(course: any): void {
    const action$ = course.isDeleted
      ? this.adminService.enableCourse(course.id)
      : this.adminService.disableCourse(course.id);

    action$.subscribe(() => {
      const updatedCourses = this.coursesSubject.value.data.map(c =>
        c.id === course.id ? { ...c, isDeleted: !c.isDeleted } : c
      );
      this.coursesSubject.next({ data: updatedCourses });
    });
  }

  toggleUserStatus(user: any): void {
    const action$ = user.isDeleted
      ? this.adminService.enableUser(user.id)
      : this.adminService.disableUser(user.id);

    action$.subscribe(() => {
      const updatedUsers = this.usersSubject.value.data.map(u =>
        u.id === user.id ? { ...u, isDeleted: !u.isDeleted } : u
      );
      this.usersSubject.next({ data: updatedUsers });
    });
  }

  trackById(_: number, item: any) {
    return item.id;
  }
}
