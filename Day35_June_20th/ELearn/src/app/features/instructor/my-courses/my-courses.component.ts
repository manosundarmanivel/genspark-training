import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InstructorService } from '../services/instructor.service';
import { catchError, map, startWith } from 'rxjs/operators';
import { of, Observable } from 'rxjs';
import { RouterModule } from '@angular/router';

interface UploadedFile {
  id: string;
  fileName: string;
  topic: string;
  description: string;
}

interface Course {
thumbnailUrl: any;
  id: string;
  title: string;
  description: string;
  instructorId: string;
  uploadedFiles: UploadedFile[];
}

interface CourseState {
  loading: boolean;
  error: string;
  data: Course[];
}

@Component({
  selector: 'app-my-courses',
  standalone: true,
  imports: [CommonModule,RouterModule],
  templateUrl: './my-courses.component.html',
})
export class MyCoursesComponent {
  courses$!: Observable<CourseState>;

  constructor(private instructorService: InstructorService) {
    this.loadCourses();
  }

  loadCourses() {
    this.courses$ = this.instructorService.getInstructorCourses(1, 100).pipe(
      map(data => ({
        loading: false,
        error: '',
        data
      })),
      startWith({
        loading: true,
        error: '',
        data: []
      }),
      catchError(err =>
        of({
          loading: false,
          error: err.message || 'Failed to fetch courses',
          data: []
        })
      )
    );
  }
}
