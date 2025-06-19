import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { ProductModel } from '../models/product';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private cartItems: ProductModel[] = [];
  private cartItemsSubject = new BehaviorSubject<ProductModel[]>([]);
  cartItems$ = this.cartItemsSubject.asObservable();

  addToCart(product: ProductModel) {
    this.cartItems.push(product);
    this.cartItemsSubject.next(this.cartItems);
  }

  getCartItems(): ProductModel[] {
    return this.cartItems;
  }
}