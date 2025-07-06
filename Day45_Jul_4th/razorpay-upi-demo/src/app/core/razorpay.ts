import { Injectable } from '@angular/core';

declare var Razorpay: any;

@Injectable({ providedIn: 'root' })
export class RazorpayService {
  openCheckout(data: any, callback: (status: string, paymentId?: string) => void) {
    if (typeof Razorpay === 'undefined') {
      alert('Razorpay SDK not loaded!');
      return;
    }

    const options = {
      key: 'rzp_test_b3ElOhVkNMsw8i',
      amount: data.amount * 100,
      currency: 'INR',
      name: data.name,
      description: 'Test Payment',
      prefill: {
        name: data.name,
        email: data.email,
        contact: data.contact
      },
      method: {
        upi: true
      },
      handler: (response: any) => {
        console.log('Payment Success:', response);
        callback('success', response.razorpay_payment_id);
      },
      modal: {
        ondismiss: () => {
          console.log('Payment Cancelled');
          callback('cancel');
        }
      }
    };

    const rzp = new Razorpay(options);
    rzp.open();
  }
}
