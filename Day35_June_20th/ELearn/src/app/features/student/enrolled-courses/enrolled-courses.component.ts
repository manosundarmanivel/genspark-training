import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { StudentService } from '../services/student.service';
import { RouterModule } from '@angular/router';
import { catchError, map, of, startWith } from 'rxjs';

@Component({
  selector: 'app-enrolled-courses',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './enrolled-courses.component.html',
})
export class EnrolledCoursesComponent {
  private studentService = inject(StudentService);

  enrolledCourses$ = this.studentService.getEnrolledCourses().pipe(
    map((courses) => ({ courses, error: null, loading: false })),
    catchError((err) =>
      of({
        courses: [],
        error: err.message || 'Failed to load enrolled courses.',
        loading: false,
      })
    ),
    startWith({ courses: [], error: null, loading: true })
  );
}
