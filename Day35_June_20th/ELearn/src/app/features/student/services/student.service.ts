import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { catchError, forkJoin, map, Observable, of, throwError } from 'rxjs';
import { switchMap, tap } from 'rxjs/operators';


@Injectable({ providedIn: 'root' })
export class StudentService {
  private baseUrl = 'http://localhost:5295/api/v1/courses';

  constructor(private http: HttpClient) {}





getEnrolledCourseDetails(): Observable<any[]> {
  console.log('Fetching enrolled courses with full details...');

  return this.http.get<any>('http://localhost:5295/api/v1/enrollments').pipe(
    tap(res => console.log('Enrolled course list:', res)),
    map(res => res?.data ?? []), // Extract array of courses
    switchMap((courses: any[]) => {
      const courseDetailRequests = courses.map(course =>
        this.http.get<any>(`${this.baseUrl}/${course.id}`).pipe(
          map(res => res?.data),
          catchError(err => {
            console.error(`Error fetching course ${course.id}`, err);
            return [null]; // Skip failed course
          })
        )
      );
      return forkJoin(courseDetailRequests); // Wait for all course details
    }),
    map(results => results.filter(course => !!course)), // Remove nulls
    catchError(err => {
      console.error('Failed to load enrolled course details', err);
      return throwError(() => new Error('Could not load course details'));
    })
  );
}


  getAllCourses() {
    console.log('Calling GetAllCourses...');
    return this.http.get<any>(this.baseUrl).pipe(
      tap(response => console.log('GetAllCourses API Response:', response)),
      map(res => res?.data ?? []),
      catchError(err => {
        console.error('Error fetching courses:', err);
        return throwError(() => new Error('Failed to fetch courses'));
      })
    );
  }

  searchCourses(query: string) {
    if (!query || query.trim().length < 2) {
      console.warn('Search query too short:', query);
      return of([]);
    }
    console.log('Searching courses with query:', query);
    return this.http.get<any>(`http://localhost:5295/api/v1/courses/search?query=${query}`).pipe(
      tap(res => console.log('SearchCourses Response:', res)),
      map(res => res?.data ?? []),
      catchError(err => {
        console.error('Error during course search:', err);
        return of([]);
      })
    );
  }

  getCourseById(courseId: string): Observable<any> {
    console.log('Fetching course by ID:', courseId);
    return this.http.get<any>(`${this.baseUrl}/${courseId}`).pipe(
      tap(res => console.log(`Course ${courseId} fetched:`, res)),
      catchError(this.handleError)
    );
  }

  enrollInCourse(courseId: string) {
    console.log('Enrolling in course:', courseId);
    return this.http.post(`http://localhost:5295/api/v1/enrollments/${courseId}`, {}).pipe(
      tap(() => console.log(`Enrolled in course: ${courseId}`)),
      catchError(err => {
        console.error('Enrollment failed:', err);
        return throwError(() => err);
      })
    );
  }

  getEnrolledCourses() {
    console.log('Fetching enrolled courses...');
    return this.http.get<any>('http://localhost:5295/api/v1/enrollments').pipe(
      tap(res => console.log('Enrolled courses:', res)),
      map(res => res?.data ?? []),
      catchError(err => {
        console.error('Error fetching enrolled courses:', err);
        return throwError(() => new Error('Failed to load enrolled courses'));
      })
    );
  }

  markFileAsCompleted(fileId: string) {
    console.log('Marking file as completed:', fileId);
    return this.http.post(`http://localhost:5295/api/v1/progress/complete/${fileId}`, {}).pipe(
      tap(() => console.log(`File ${fileId} marked as completed.`)),
      catchError(err => {
        console.error(`Failed to mark file ${fileId} as completed:`, err);
        return throwError(() => new Error('Failed to update progress'));
      })
    );
  }

  private handleError(error: HttpErrorResponse) {
    let message = 'An unknown error occurred.';
    if (error.error instanceof ErrorEvent) {
      message = `Client Error: ${error.error.message}`;
    } else {
      message = `Server Error (${error.status}): ${error.message}`;
      if (error.error?.message) {
        message += ` - ${error.error.message}`;
      }
    }
    console.error('HTTP Error:', message);
    return throwError(() => new Error(message));
  }
}
