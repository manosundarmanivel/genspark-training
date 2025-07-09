
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';


declare var Razorpay: any;

@Injectable({ providedIn: 'root' })
export class RazorpayService {
openCheckout(options: any, onFailure: (response: any) => void): void {
    const rzp = new Razorpay(options);

   
    rzp.on('payment.failed', (response: any) => {
      onFailure(response.error);
    });

    rzp.open();
  }


  private readonly apiUrl = 'http://localhost:5295/api/v1/Order';

  constructor(private http: HttpClient) {}

  createRazorpayOrder(amount: number) {
    return this.http.post<{ orderId: string }>(`${this.apiUrl}/create-order`, { amount });
  
}
}
