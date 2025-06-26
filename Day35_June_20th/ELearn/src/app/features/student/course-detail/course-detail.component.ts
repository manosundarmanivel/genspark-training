import { Component, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { StudentService } from '../services/student.service';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';
import { switchMap, map, catchError, of } from 'rxjs';
import { tap } from 'rxjs/operators';


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
  selectedVideo: any = null;


course$ = this.route.paramMap.pipe(
  switchMap(paramMap => {
    const courseId = paramMap.get('courseId');
    if (!courseId) {
      console.warn('Course ID is missing in route.');
      return of({ error: 'Course ID missing', data: null });
    }

    return this.studentService.getCourseById(courseId).pipe(
      map(res => ({ data: res.data, error: null })),
      catchError(err => {
        console.error('Error loading course:', err);
        return of({ error: err.message || 'Failed to load course details.', data: null });
      })
    );
  }),
  tap(result => {
    if (result.error) {
      console.warn('Course load failed:', result.error);
    } else {
      console.log('Course data loaded:', result.data);
    }
  })
);

  

getVideoUrl(fileName: string): SafeResourceUrl {
  const url = `http://localhost:5295/api/v1/courses/stream/${fileName}`;
  return this.sanitizer.bypassSecurityTrustResourceUrl(url);
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

  markAsCompleted(fileId: string): void {
  this.studentService.markFileAsCompleted(fileId).subscribe({
    next: () => {
      // Update the UI without reload
      const current = this.selectedVideo;
      if (current?.id === fileId) current.isCompleted = true;
      this.course$.pipe(tap(result => {
        result.data.uploadedFiles.forEach((f: any) => {
          if (f.id === fileId) f.isCompleted = true;
        });
      })).subscribe();

      alert('Marked as completed!');
    },
    error: err => {
      console.error('Failed to mark as completed:', err);
      alert('Failed to mark as completed. Please try again.');
    }
  });
}

}
