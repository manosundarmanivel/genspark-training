import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../auth/auth.service';
import { StudentService } from '../services/student.service';
import { Observable, tap, map } from 'rxjs';

@Component({
  selector: 'app-student-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './student-dashboard.component.html'
})
export class StudentDashboardComponent {
  enrolledCourses$: Observable<any[]>;
  totalCourses$: Observable<number>;
  completedCourses$: Observable<number>;

  constructor(private auth: AuthService, private studentService: StudentService) {
    this.enrolledCourses$ = this.studentService.getEnrolledCourseDetails().pipe(
      tap(data => console.log('Enrolled course details:', data))
    );

    this.totalCourses$ = this.enrolledCourses$.pipe(map(courses => courses.length));
    this.completedCourses$ = this.enrolledCourses$.pipe(
      map(courses => courses.filter(c => c.isCompleted).length)
    );
  }

  logout() {
    this.auth.logout();
  }
}
