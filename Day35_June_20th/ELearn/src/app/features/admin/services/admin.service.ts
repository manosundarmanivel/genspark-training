import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { forkJoin, Observable, of } from 'rxjs';
import { map, switchMap, tap, catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  private readonly baseUrl = 'http://localhost:5295/api/v1/admin';
  private readonly baseUrlCourse = 'http://localhost:5295/api/v1/courses';

  constructor(private http: HttpClient) {}

  // Fetch users and fully detailed courses
getUsersAndDetailedCourses(): Observable<{ users: any[]; courses: any[] }> {
  return forkJoin({
    users: this.http.get(`${this.baseUrl}/users`).pipe(
      tap(res => console.log('[GET Users]', res))
    ),
    courses: this.http.get(`${this.baseUrl}/courses`).pipe(
      tap(res => console.log('[GET Courses]', res))
    )
  }).pipe(
    switchMap(({ users, courses }: any) => {
      const courseList = courses?.data || [];

      const detailedCourses$ = courseList.length > 0
        ? forkJoin(
            courseList.map((c: any) =>
              this.getCourseById(c.id).pipe(
                catchError(err => {
                  console.error(`Failed to fetch course ${c.id}`, err);
                  return of(c); // fallback to original
                })
              )
            )
          )
        : of([]);

      return (detailedCourses$ as Observable<any[]>).pipe(
        map((detailedCourses: any[]) => ({
          users: users?.data || [],
          courses: detailedCourses
        }))
      );
    })
  );
}


  // Original combined API for raw users and course list
  getAllUsersAndCourses(): Observable<{ users: any; courses: any }> {
    return forkJoin({
      users: this.http.get(`${this.baseUrl}/users`).pipe(
        tap(res => console.log('[GET Users]', res))
      ),
      courses: this.http.get(`${this.baseUrl}/courses`).pipe(
        tap(res => console.log('[GET Courses]', res))
      )
    });
  }

  // Get full course details by ID
  getCourseById(courseId: string): Observable<any> {
    console.log('Fetching course by ID:', courseId);
    return this.http.get<any>(`${this.baseUrlCourse}/${courseId}`).pipe(
      tap(res => console.log(`Course ${courseId} fetched:`, res))
    );
  }

  // -------- USER MANAGEMENT --------
  getAllUsers(): Observable<any> {
    return this.http.get(`${this.baseUrl}/users`).pipe(
      tap(res => console.log('[GET Users]', res))
    );
  }

  getUserById(id: string): Observable<any> {
    return this.http.get(`${this.baseUrl}/users/${id}`).pipe(
      tap(res => console.log(`[GET User ${id}]`, res))
    );
  }

  enableUser(id: string): Observable<any> {
    return this.http.post(`${this.baseUrl}/users/${id}/enable`, {}).pipe(
      tap(res => console.log(`[Enable User ${id}]`, res))
    );
  }

  disableUser(id: string): Observable<any> {
    return this.http.post(`${this.baseUrl}/users/${id}/disable`, {}).pipe(
      tap(res => console.log(`[Disable User ${id}]`, res))
    );
  }

  // -------- COURSE MANAGEMENT --------
  getAllCourses(): Observable<any> {
    return this.http.get(`${this.baseUrl}/courses`).pipe(
      tap(res => console.log('[GET Courses]', res))
    );
  }

  enableCourse(id: string): Observable<any> {
    return this.http.post(`${this.baseUrl}/courses/${id}/enable`, {}).pipe(
      tap(res => console.log(`[Enable Course ${id}]`, res))
    );
  }

  disableCourse(id: string): Observable<any> {
    return this.http.post(`${this.baseUrl}/courses/${id}/disable`, {}).pipe(
      tap(res => console.log(`[Disable Course ${id}]`, res))
    );
  }
}
