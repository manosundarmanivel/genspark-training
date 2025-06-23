import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { catchError, map, throwError } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class InstructorService {
  private courseUrl = 'http://localhost:5295/api/v1/courses';
  private fileUploadUrl = 'http://localhost:5295/api/v1/files/upload';
  private instructorCoursesUrl = 'http://localhost:5295/api/v1/courses/instructor';

  constructor(private http: HttpClient) {}

  addCourse(data: { title: string; description: string }) {
    return this.http.post<any>(this.courseUrl, data).pipe(catchError(this.handleError));
  }

  uploadFile(formData: FormData) {
    return this.http.post<any>(this.fileUploadUrl, formData).pipe(catchError(this.handleError));
  }

  getInstructorCourses(page: number, pageSize: number) {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    return this.http.get<any>(this.instructorCoursesUrl, { params }).pipe(
      map(response => response?.data ?? []),
      catchError(this.handleError)
    );
  }

  private handleError(error: HttpErrorResponse) {
    let errorMsg = 'An unknown error occurred';
    if (error.error instanceof ErrorEvent) {
      errorMsg = `Client Error: ${error.error.message}`;
    } else {
      errorMsg = `Server Error (${error.status}): ${error.message}`;
      if (error.error?.message) errorMsg += ` - ${error.error.message}`;
    }
    console.error('HTTP Error:', errorMsg);
    return throwError(() => new Error(errorMsg));
  }
}
