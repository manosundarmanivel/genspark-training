import { Component, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { StudentService } from '../services/student.service';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';
import { switchMap, map, catchError, of } from 'rxjs';

@Component({
  selector: 'app-course-detail',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './course-detail.component.html',
})
export class CourseDetailComponent {
  private route = inject(ActivatedRoute);
  private studentService = inject(StudentService);
  private sanitizer = inject(DomSanitizer);
  private router = inject(Router);

  course$ = this.route.paramMap.pipe(
    switchMap(paramMap => {
      const courseId = paramMap.get('courseId');
      if (!courseId) {
        return of({ error: 'Course ID missing', data: null });
      }

      return this.studentService.getCourseById(courseId).pipe(
        map(res => ({ data: res.data, error: null })),
        catchError(err => of({ error: err.message || 'Failed to load course details.', data: null }))
      );
    })
  );

  getVideoUrl(fileName: string): SafeResourceUrl {
    const path = `/assets/videos/${fileName}`;
    return this.sanitizer.bypassSecurityTrustResourceUrl(path);
  }

  enroll(courseId: string): void {
    this.studentService.enrollInCourse(courseId).subscribe({
      next: () => {
        window.confirm('Enrolled successfully! You can now access full content.');
        this.router.navigate(['/student-dashboard/enrolled']);
      },
      error: err => {
        alert(`Enrollment failed: ${err.error?.message || err.message}`);
      }
    });
  }
}
