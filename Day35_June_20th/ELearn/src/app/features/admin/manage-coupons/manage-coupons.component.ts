import { Component, effect, computed, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { AdminService } from '../services/admin.service';
import { ToastrService } from 'ngx-toastr';
import { formatDate } from '@angular/common';
import { tap, switchMap } from 'rxjs/operators';
import { of } from 'rxjs';

@Component({
  standalone: true,
  selector: 'app-manage-coupons',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './manage-coupons.component.html',
})
export class ManageCouponsComponent {
  coupons = signal<any[]>([]);
  isEditing = signal(false);
  editingId = signal<string | null>(null);

  couponForm!: FormGroup;

  buttonLabel = computed(() =>
    this.isEditing() ? 'Update Coupon' : 'Create Coupon'
  );

  constructor(
    private adminService: AdminService,
    private fb: FormBuilder,
    private toastr: ToastrService
  ) {
    this.initForm();
    this.loadCoupons();
  }

  initForm() {
    this.couponForm = this.fb.group({
      code: ['', Validators.required],
      discountAmount: [null],
      discountPercentage: [null],
      expiryDate: ['', Validators.required],
      usageLimit: [1, [Validators.required, Validators.min(1)]],
      isActive: [true],
    });
  }

loadCoupons() {
  this.adminService
    .getAllCoupons()
    .pipe(
      tap((res) => {
        const updatedCoupons = res.map((coupon) => {
          const now = new Date();
          const expiry = new Date(coupon.expiryDate);
          const isExpired = expiry < now;
          const isOverused = coupon.timesUsed >= coupon.usageLimit;

          // Disable the coupon if expired or overused
          if ((isExpired || isOverused) && coupon.isActive) {
            this.adminService.setCouponStatus(coupon.id, false).subscribe({
              next: () => console.log(`Auto-disabled coupon: ${coupon.code}`),
              error: () => console.warn(`Failed to auto-disable coupon: ${coupon.code}`)
            });
            return { ...coupon, isActive: false }; // update locally too
          }

          return coupon;
        });

        this.coupons.set(updatedCoupons);
      }),
      tap({ error: () => this.toastr.error('Failed to load coupons') })
    )
    .subscribe();
}

isCouponExpiredOrOverused(coupon: any): boolean {
  const now = new Date();
  const expiry = new Date(coupon.expiryDate);
  return expiry < now || coupon.timesUsed >= coupon.usageLimit;
}


  submitForm() {
    if (this.couponForm.invalid) return;

    const formValue = this.couponForm.value;

    // format expiry date to full ISO string (UTC)
    const data = {
      ...formValue,
      expiryDate: new Date(formValue.expiryDate).toISOString(),
    };

    const request$ =
      this.isEditing() && this.editingId()
        ? this.adminService.updateCoupon(this.editingId()!, data).pipe(
            tap(() => this.toastr.success('Coupon updated'))
          )
        : this.adminService.createCoupon(data).pipe(
            tap(() => this.toastr.success('Coupon created'))
          );

    request$
      .pipe(
        switchMap(() => {
          this.resetForm();
          return this.adminService.getAllCoupons();
        }),
        tap((res) => this.coupons.set(res)),
        tap({ error: () => this.toastr.error('Failed to reload coupons') })
      )
      .subscribe();
  }

  editCoupon(coupon: any) {
    this.isEditing.set(true);
    this.editingId.set(coupon.id);
    this.couponForm.patchValue({
      code: coupon.code,
      discountAmount: coupon.discountAmount,
      discountPercentage: coupon.discountPercentage,
     
      expiryDate: this.formatDateOnly(coupon.expiryDate),
      usageLimit: coupon.usageLimit,
      isActive: coupon.isActive,
    });
  }

  toggleStatus(coupon: any) {
    this.adminService
      .setCouponStatus(coupon.id, !coupon.isActive)
      .pipe(
        tap(() =>
          this.toastr.success(`Coupon ${!coupon.isActive ? 'enabled' : 'disabled'}`)
        ),
        switchMap(() => this.adminService.getAllCoupons()),
        tap((res) => this.coupons.set(res)),
        tap({ error: () => this.toastr.error('Failed to update status') })
      )
      .subscribe();
  }

 
  formatDateOnly(isoString: string): string {
    const date = new Date(isoString);
    return date.toISOString().split('T')[0];
  }

  resetForm() {
    this.isEditing.set(false);
    this.editingId.set(null);
    this.couponForm.reset({ isActive: true, usageLimit: 1 });
  }
}
