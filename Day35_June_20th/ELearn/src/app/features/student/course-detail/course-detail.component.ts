import { Component, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { StudentService } from '../services/student.service';
import { AdminService } from '../../admin/services/admin.service';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';
import { switchMap, map, catchError, of, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';
import { ToastrService } from 'ngx-toastr';
import { ChangeDetectorRef } from '@angular/core';
import { RazorpayService } from '../../../shared/services/razorpay.service';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-course-detail',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './course-detail.component.html',
})
export class CourseDetailComponent {
  private route = inject(ActivatedRoute);
  private studentService = inject(StudentService);
  private sanitizer = inject(DomSanitizer);
  private router = inject(Router);
  private toastr = inject(ToastrService)
  private cdr = inject(ChangeDetectorRef)
  private razorpay = inject(RazorpayService);
  private fb = inject(FormBuilder);
  private adminService = inject(AdminService)

  checkoutVisible = false;
  checkoutForm!: FormGroup;
  selectedCourse: any;
  finalAmount = 0;
  appliedCouponId: string | null = null;

  ngOnInit() {
    this.checkoutForm = this.fb.group({
      coupon: [''],
    });
  }


  private couponSubject = new BehaviorSubject<any>(null);
  coupons$ = this.couponSubject.asObservable();

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

  isLessonLocked(fileIndex: number): boolean {
    const current = this.courseSubject.value;
    if (!current || !Array.isArray(current.uploadedFiles)) return false;

    // All lessons before this index must be completed
    for (let i = 0; i < fileIndex; i++) {
      if (!current.uploadedFiles[i].isCompleted) return true;
    }

    return false;
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

  openCheckoutModal(course: any) {
    this.selectedCourse = course;
    this.finalAmount = course.price;
    this.checkoutForm.reset();
    this.checkoutVisible = true;
  }

  applyCoupon() {
    const code = this.checkoutForm.value.coupon;
    if (!code) return;

    this.adminService.validateCoupon(code).subscribe({
      next: (coupon) => {
        const { discountAmount, discountPercentage } = coupon;
        let discounted = this.selectedCourse.price;

        if (discountPercentage)
          discounted -= (discountPercentage / 100) * discounted;
        else if (discountAmount)
          discounted -= discountAmount;

        this.finalAmount = Math.max(0, Math.floor(discounted));

        this.toastr.success(`Coupon applied! New amount: ₹${this.finalAmount}`);
        this.appliedCouponId = coupon.id;
        this.cdr.detectChanges();
        console.log('Coupon applied:', this.appliedCouponId);
        this.couponSubject.next(coupon);
      },
      error: () => {
        this.toastr.error('Invalid coupon code');
        this.finalAmount = this.selectedCourse.price;
        this.appliedCouponId = null;
        this.couponSubject.next(null);
      }
    });
  }


payAndEnroll() {
  this.razorpay.createRazorpayOrder(this.finalAmount).subscribe({
    next: ({ orderId }) => {
      const options = {
        key: 'rzp_test_b3ElOhVkNMsw8i',
        amount: this.finalAmount * 100,
        currency: 'INR',
        name: 'Elearn',
        description: `Enroll in ${this.selectedCourse.title}`,
        order_id: orderId,
        handler: (response: any) => {
          this.toastr.success('Payment successful!');
          this.checkoutVisible = false;

          const transactionPayload = {
            paymentId: response.razorpay_payment_id,
            orderId: response.razorpay_order_id,
            userId: localStorage.getItem('userId')!,
            courseId: this.selectedCourse.id,
            amount: this.finalAmount,
            currency: 'INR',
            status: 'success',
          };

          this.adminService.addTransaction(transactionPayload).subscribe({
            next: () => {
              console.log('Transaction recorded');
              this.enroll(this.selectedCourse.id);
            },
            error: (err) => {
              console.error('Failed to record transaction', err);
              this.enroll(this.selectedCourse.id);
            }
          });

          if (this.appliedCouponId) {
            this.adminService.incrementCouponUsage(this.appliedCouponId).subscribe();
          }
        },
        prefill: {
          email: 'student@example.com',
        },
        theme: {
          color: '#6366F1'
        }
      };

     
      this.razorpay.openCheckout(options, (error: any) => {
        this.toastr.error('Payment failed: ' + error.description);
        console.error('Payment failed', error);


        const failedTransactionPayload = {
          paymentId: error.metadata?.payment_id || 'N/A',
          orderId: error.metadata?.order_id || orderId,
          userId: localStorage.getItem('userId')!,
          courseId: this.selectedCourse.id,
          amount: this.finalAmount,
          currency: 'INR',
          status: 'failed',
        };

        this.adminService.addTransaction(failedTransactionPayload).subscribe({
          next: () => console.log('Failed transaction recorded'),
          error: (err) => console.error('Error logging failed transaction', err)
        });
      });
    },
    error: () => this.toastr.error('Failed to initiate payment')
  });
}




}
