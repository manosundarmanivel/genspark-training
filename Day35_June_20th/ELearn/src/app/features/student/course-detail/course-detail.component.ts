import { Component, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { StudentService } from '../services/student.service';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';
import { switchMap, map, catchError, of, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';
import { ToastrService } from 'ngx-toastr';
import { ChangeDetectorRef } from '@angular/core';

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
  private toastr= inject(ToastrService)
   private cdr = inject(ChangeDetectorRef )
  

  private courseSubject = new BehaviorSubject<any>(null);
  course$ = this.courseSubject.asObservable().pipe(
    map(data => ({ data, error: null })),
    catchError(err => of({ error: err.message, data: null }))
  );

  selectedVideo: any = null;

  constructor() {
    this.loadCourse();
  }

  private loadCourse(): void {
    this.route.paramMap
      .pipe(
        switchMap(paramMap => {
          const courseId = paramMap.get('courseId');
          if (!courseId) return of(null);
          return this.studentService.getCourseById(courseId);
        }),
        catchError(err => {
          console.error('Failed to load course', err);
          return of(null);
        }),
        tap(course => {
          this.courseSubject.next(course?.data || null);
          if (course?.data?.firstUploadedFile) {
            this.selectedVideo = course.data.firstUploadedFile;
          }
        })
      )
      .subscribe();
  }

  getVideoUrl(fileName: string): SafeResourceUrl {
    const url = `http://localhost:5295/api/v1/courses/stream/${fileName}`;
    return this.sanitizer.bypassSecurityTrustResourceUrl(url);
  }

  enroll(courseId: string): void {
    this.studentService.enrollInCourse(courseId).subscribe({
      next: () => {
        this.toastr.success('Enrolled successfully!');
        this.cdr.detectChanges(); 
        this.router.navigate(['/student-dashboard/enrolled']);
      },
      error: err => {
        

        this.toastr.error(`Enrollment failed: ${err.error?.message || err.message}`);
        this.cdr.detectChanges(); 
        
      }
    });
  }

  markAsCompleted(fileId: string): void {
    this.studentService.markFileAsCompleted(fileId).subscribe({
      next: () => {
        const current = this.courseSubject.value;
        if (!current) return;

        // update selectedVideo if it's the same
        if (this.selectedVideo?.id === fileId) {
          this.selectedVideo.isCompleted = true;
        }

        const updatedFiles = current.uploadedFiles.map((f: any) =>
          f.id === fileId ? { ...f, isCompleted: true } : f
        );

        const updatedCourse = { ...current, uploadedFiles: updatedFiles };
        this.courseSubject.next(updatedCourse);
      },
      error: err => {
        console.error('Failed to mark as completed', err);
      
         this.toastr.error('Failed to mark as completed.');
            this.cdr.detectChanges(); 
      }
    });
  }
}
