import { Component } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { RazorpayService } from '../../core/razorpay';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-payment-form',
  templateUrl: './payment-form.html',
  styleUrls: ['./payment-form.css'],
  imports:[FormsModule, CommonModule, ReactiveFormsModule]
})
export class PaymentFormComponent {
  paymentForm: FormGroup;
  message: string = '';
  isLoading = false;

  constructor(private fb: FormBuilder, private razorpayService: RazorpayService) {
    this.paymentForm = this.fb.group({
      amount: [null, [Validators.required, Validators.min(1)]],
      name: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      contact: ['', [Validators.required, Validators.pattern(/^\d{10}$/)]]
    });
  }

  payNow() {
    if (this.paymentForm.invalid) {
      return;
    }

    this.isLoading = true;

    this.razorpayService.openCheckout(this.paymentForm.value, (status, paymentId?) => {
      this.isLoading = false;
      this.message = status === 'success'
        ? `Payment Success! Payment ID: ${paymentId}`
        : 'Payment Failed or Cancelled';
    });
  }
}
