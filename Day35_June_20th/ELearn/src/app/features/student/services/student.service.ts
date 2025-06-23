import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { catchError, map, Observable, of, throwError } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class StudentService {
  private baseUrl = 'http://localhost:5295/api/v1/courses';

  constructor(private http: HttpClient) {}

  getAllCourses() {
    return this.http.get<any>(this.baseUrl).pipe(
      map(res => res?.data ?? []),
      catchError(err => {
        console.error('Error fetching courses:', err);
        return throwError(() => new Error('Failed to fetch courses'));
      })
    );
  }

  searchCourses(query: string) {
  if (!query || query.trim().length < 2) return of([]);
  return this.http.get<any>(`http://localhost:5295/api/v1/courses/search?query=${query}`).pipe(
    map(res => res?.data ?? []),
    catchError(() => of([]))
  );
}


 getCourseById(courseId: string): Observable<any> {
    return this.http
      .get<any>(`${this.baseUrl}/${courseId}`)
      .pipe(catchError(this.handleError));
  }

 
enrollInCourse(courseId: string) {
  return this.http.post(`http://localhost:5295/api/v1/enrollments/${courseId}`, {})
    .pipe(
      catchError(err => {
        console.error('Enrollment failed:', err);
        return throwError(() => err);
      })
    );
}

getEnrolledCourses() {
  return this.http.get<any>('http://localhost:5295/api/v1/enrollments').pipe(
    map(res => res?.data ?? []), 
    catchError(err => {
      console.error('Error fetching enrolled courses:', err);
      return throwError(() => new Error('Failed to load enrolled courses'));
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
    return throwError(() => new Error(message));
  }


}
