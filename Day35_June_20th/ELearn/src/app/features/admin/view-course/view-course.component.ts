import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { switchMap, catchError, map, startWith, tap } from 'rxjs/operators';
import { Observable, of, BehaviorSubject } from 'rxjs';

@Component({
  selector: 'app-view-course',
  standalone: true,
  imports: [CommonModule, HttpClientModule],
  templateUrl: './view-course.component.html',
})
export class ViewCourseComponent implements OnInit {
  courseId: string = '';
  refresh$ = new BehaviorSubject<void>(undefined);

  students$: Observable<{ loading: boolean; error: string | null; data: any[] }> = of({ loading: true, error: null, data: [] });

  constructor(private route: ActivatedRoute, private http: HttpClient) {}

  ngOnInit(): void {
    this.courseId = this.route.snapshot.paramMap.get('id')!;

    this.students$ = this.refresh$.pipe(
      startWith(undefined),
      switchMap(() =>
        this.http
          .get<{ success: boolean; data: any[]; message?: string }>(
            `http://localhost:5295/api/v1/courses/students/${this.courseId}`
          )
          .pipe(
            tap(res => console.log('Fetched students:', res.data)),
            map(res => ({ loading: false, error: null, data: res.data })),
            catchError(err =>
              of({
                loading: false,
                error: err?.error?.message || 'Failed to load student list.',
                data: [],
              })
            )
          )
      ),
      startWith({ loading: true, error: null, data: [] })
    );
  }

  unenrollStudent(userId: string): void {
    const url = `http://localhost:5295/api/v1/enrollments/${userId}/${this.courseId}`;
    this.http.delete(url).subscribe({
      next: () => {
        console.log(`User ${userId} unenrolled successfully.`);
        this.refresh$.next(); // Refresh student list
      },
      error: err => {
        console.error('Unenroll failed:', err);
        alert(err?.error?.message || 'Unenroll failed.');
      },
    });
  }
}
