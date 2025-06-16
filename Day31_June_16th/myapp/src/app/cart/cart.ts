import { Component, OnInit } from '@angular/core';
import { CartService } from '../service/cart.service';
import { ProductModel } from '../models/product';
import { CommonModule, CurrencyPipe } from '@angular/common';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.html',
  imports: [CurrencyPipe,CommonModule],
  styleUrl: './cart.css',
  standalone: true
})
export class Cart implements OnInit {
  cartItems: ProductModel[] = [];

  constructor(private cartService: CartService) {}

  ngOnInit() {
    this.cartService.cartItems$.subscribe(items => {
      this.cartItems = items;
    });
  }
}