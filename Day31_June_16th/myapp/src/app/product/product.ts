import { Component, Input, Output, EventEmitter } from '@angular/core';
import { ProductModel } from '../models/product';
import { CurrencyPipe } from '@angular/common';

@Component({
  selector: 'app-product',
  templateUrl: './product.html',
  imports: [CurrencyPipe],
  styleUrl: './product.css',
  standalone: true
})
export class Product {
  @Input() product: ProductModel | null = null;
  @Output() addToCart = new EventEmitter<ProductModel>();

  onAddToCart() {
    if (this.product) {
      this.addToCart.emit(this.product);
    }
  }
}