import { Component } from '@angular/core';
import { CartService } from '../cart.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-product-list',
  imports: [CommonModule],
  templateUrl: './product-list.html',
  styleUrl: './product-list.css',
 
})
export class ProductList {

   products = [
    { id: 1, name: 'Product A', img: 'assets/product1.jpeg' },
    { id: 2, name: 'Product B', img: 'assets/product2.jpeg' },
    { id: 3, name: 'Product C', img: 'assets/product3.jpeg' }
  ];

  constructor(private cartService: CartService) {}

  addToCart() {
    this.cartService.addToCart();
  }
}
