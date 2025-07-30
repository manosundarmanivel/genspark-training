import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ProductDto } from '../products/product.service';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.html',
  styleUrls: ['./cart.css'],
  imports : [CommonModule, ReactiveFormsModule]
})
export class CartComponent implements OnInit {
  cartItems: ProductDto[] = [];
  orderForm!: FormGroup;
  successMessage: string = '';

  constructor(private http: HttpClient, private fb: FormBuilder) {}

  ngOnInit(): void {
    const cartJson = sessionStorage.getItem('cart');
    this.cartItems = cartJson ? JSON.parse(cartJson) : [];

    this.orderForm = this.fb.group({
      orderName: ['Order_' + new Date().getTime(), Validators.required],
      orderDate: [new Date()],
      paymentType: ['Cash'],
      status: ['Pending'],
      customerName: ['', Validators.required],
      customerPhone: ['', Validators.required],
      customerEmail: ['', [Validators.required, Validators.email]],
      customerAddress: ['', Validators.required]
    });
  }

getTotalPrice(): number {
  return this.cartItems.reduce((total, item) => {
    const price = Number(item.price) || 0;
    const quantity = Number(item.quantity) || 1;
    return total + (price * quantity);
  }, 0);
}


  removeItem(productId: number) {
    this.cartItems = this.cartItems.filter(item => item.productId !== productId);
    sessionStorage.setItem('cart', JSON.stringify(this.cartItems));
  }

  placeOrder() {
    if (this.orderForm.invalid || this.cartItems.length === 0) return;

    const orderData = this.orderForm.value;

    this.http.post<any>('http://localhost:5228/api/orders', orderData).subscribe({
      next: (orderResponse) => {
        const orderId = orderResponse.orderID;
        console.log(orderId);

        const orderDetails = this.cartItems.map(product => ({
          orderID: orderId,
          productID: product.productId,
          price: product.price,
          quantity: product.quantity
        }));

       const detailRequests = orderDetails.map(detail =>
  this.http.post('http://localhost:5228/api/OrderDetails', detail)
);

Promise.all(detailRequests.map(req => req.toPromise()))
  .then(() => {
    this.successMessage = 'Order placed successfully!';
    this.cartItems = [];
    sessionStorage.removeItem('cart');
    this.orderForm.reset();
  })
  .catch(() => {
    alert('Order created but failed to create some order details.');
  });

      },
      error: () => {
        alert('Failed to create order');
      }
    });
  }
}
