import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { forkJoin, Observable, of } from 'rxjs';
import { map, switchMap, tap, catchError } from 'rxjs/operators';


export interface CouponResponse {
  discountAmount?: number;
  discountPercentage?: number;
}

export interface Transaction {
  id: string;
  paymentId: string;
  orderId: string;
  amount: number;
  currency: string;
  status: string;
  createdAt: string;

  userId: string;
  userName: string;
  courseId: string;
  courseTitle: string;
}

export interface EnrollmentStats {
  date: string;
  enrollmentCount: number;
}



@Injectable({
  providedIn: 'root'
})


export class AdminService {
  private readonly baseUrl = 'http://localhost:5295/api/v1/admin';
  private readonly baseUrlCourse = 'http://localhost:5295/api/v1/courses';
  private readonly baseUrlCoupon = 'http://localhost:5295/api/v1/coupons';
  private readonly baseUrlTransaction = 'http://localhost:5295/api/v1/transactions';

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


  // Get all coupons
getAllCoupons(): Observable<any[]> {
  return this.http.get<any[]>(`${this.baseUrlCoupon}`).pipe(
    tap(res => console.log('[GET Coupons]', res))
  );
}

// Get coupon by ID
getCouponById(id: string): Observable<any> {
  return this.http.get(`${this.baseUrlCoupon}/${id}`).pipe(
    tap(res => console.log(`[GET Coupon ${id}]`, res))
  );
}

// Create new coupon
createCoupon(payload: {
  code: string;
  discountAmount?: number;
  discountPercentage?: number;
  expiryDate: string;
  usageLimit: number;
}): Observable<any> {
  return this.http.post(`${this.baseUrlCoupon}`, payload).pipe(
    tap(res => console.log('[CREATE Coupon]', res))
  );
}

// Update coupon
updateCoupon(id: string, payload: {
  code: string;
  discountAmount?: number;
  discountPercentage?: number;
  expiryDate: string;
  usageLimit: number;
  isActive: boolean;
}): Observable<any> {
  return this.http.put(`${this.baseUrlCoupon}/${id}`, payload).pipe(
    tap(res => console.log(`[UPDATE Coupon ${id}]`, res))
  );
}

// Enable/Disable coupon
setCouponStatus(id: string, isActive: boolean): Observable<any> {
  const action = isActive ? 'enable' : 'disable';
  return this.http.patch(`${this.baseUrlCoupon}/${id}/${action}`, {}).pipe(
    tap(res => console.log(`[${action.toUpperCase()} Coupon ${id}]`, res))
  );
}



validateCoupon(code: string): Observable<any> {
    return this.http.get<any>(`${this.baseUrlCoupon}/validate?code=${code}`);
  }

  // Increment coupon usage (TimesUsed++)
incrementCouponUsage(id: string): Observable<any> {
  return this.http.patch(`${this.baseUrlCoupon}/${id}/increment-usage`, {}).pipe(
    tap(res => console.log(`[INCREMENT Coupon Usage ${id}]`, res)),
    catchError(err => {
      console.error(`Failed to increment usage for coupon ${id}`, err);
      return of(null); // or throwError if you want it to propagate
    })
  );
}

//Fetch all transactions (with user and course names)
  getAllTransactions(): Observable<Transaction[]> {
    return this.http.get<Transaction[]>(this.baseUrlTransaction).pipe(
      tap(res => console.log('[GET Transactions]', res))
    );
  }

  // Add a new transaction
  addTransaction(payload: {
    paymentId: string;
    orderId: string;
    userId: string;
    courseId: string;
    amount: number;
    currency: string;
    status: string;
  }): Observable<any> {
    return this.http.post(this.baseUrlTransaction, payload).pipe(
      tap(res => console.log('[ADD Transaction]', res)),
      catchError(err => {
        console.error('Failed to add transaction', err);
        return of(null);
      })
    );
  }

  // Get daily enrollment stats (e.g., last 7 days)
getDailyEnrollmentStats(pastDays: number): Observable<EnrollmentStats[]> {
  return this.http
    .get<EnrollmentStats[]>(`http://localhost:5295/api/v1/enrollments/analytics/daily-enrollments?days=${pastDays}`)
    .pipe(
      tap(res => console.log(`[GET Daily Enrollment Stats]`, res)),
      catchError(err => {
        console.error('Failed to fetch daily enrollment stats', err);
        return of([]); // fallback to empty list
      })
    );
}



}

