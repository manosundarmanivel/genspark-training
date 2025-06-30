import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { catchError, map, Observable, throwError } from 'rxjs';
import { tap } from 'rxjs/operators'; 

@Injectable({ providedIn: 'root' })
export class InstructorService {
  private courseUrl = 'http://localhost:5295/api/v1/courses';
  private fileUploadUrl = 'http://localhost:5295/api/v1/files/upload';
  private instructorCoursesUrl = 'http://localhost:5295/api/v1/courses/instructor';

  constructor(private http: HttpClient) {}

addCourse(formData: FormData): Observable<any> {
    console.log('submitCourse called');
  return this.http.post<any>(this.courseUrl, formData).pipe(
    catchError(this.handleError) 
  );
}




  uploadFile(formData: FormData) {
    return this.http.post<any>(this.fileUploadUrl, formData).pipe(catchError(this.handleError));
  }



getInstructorCourses(page: number, pageSize: number) {
  const params = new HttpParams()
    .set('page', page.toString())
    .set('pageSize', pageSize.toString());

  return this.http.get<any>(this.instructorCoursesUrl, { params }).pipe(
    tap(response => console.log('Instructor Courses API Response:', response)), 
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



updateFile(fileId: string, formData: FormData) {
  return this.http.put<any>(`http://localhost:5295/api/v1/files/${fileId}`, formData).pipe(catchError(this.handleError));
}


get(url: string) {
  return this.http.get<any>(url).pipe(catchError(this.handleError));
}


getCourseById(courseId: string) {
  return this.http.get<any>(`http://localhost:5295/api/v1/courses/${courseId}`).pipe(map(res => res.data));
}
getCourseByIdEdit(courseId: string) {
  return this.http.get<any>(`http://localhost:5295/api/v1/courses/instructor/${courseId}`).pipe(map(res => res.data));
}

getCourseFiles(courseId: string) {
  return this.http.get<any>(`http://localhost:5295/api/v1/student/files/${courseId}`).pipe(map(res => res.data));
}

updateCourseFile(fileId: string, formData: FormData) {
  return this.http.put<any>(`http://localhost:5295/api/v1/files/${fileId}`, formData)
    .pipe(catchError(this.handleError));
}


updateCourse(courseId: string, data: any) {
  return this.http.put<any>(`http://localhost:5295/api/v1/courses/${courseId}`, data);
}



}
