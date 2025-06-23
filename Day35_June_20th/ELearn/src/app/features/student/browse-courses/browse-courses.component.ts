import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { StudentService } from '../services/student.service';
import { Observable, of } from 'rxjs';
import { catchError, map, startWith } from 'rxjs/operators';
import { RouterModule } from '@angular/router';

interface Course {
  id: string;
  title: string;
  description: string;
  instructorId: string;
  uploadedFiles: any[];
}

@Component({
  selector: 'app-browse-courses',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './browse-courses.component.html',
})
export class BrowseCoursesComponent {
  private studentService = inject(StudentService);

  coursesState$: Observable<{
    courses: Course[];
    loading: boolean;
    error: string | null;
  }> = this.studentService.getAllCourses().pipe(
    map(courses => ({ courses, loading: false, error: null })),
    catchError(err =>
      of({
        courses: [],
        loading: false,
        error: err.message || 'Failed to load courses.',
      })
    ),
    startWith({ courses: [], loading: true, error: null })
  );
}
