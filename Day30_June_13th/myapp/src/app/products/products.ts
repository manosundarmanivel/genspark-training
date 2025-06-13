import { Component, OnInit } from '@angular/core';
import { ProductModel } from '../models/product';
import { ProductService } from '../service/product.service';
import { CartService } from '../service/cart.service';
import { Product } from '../product/product';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [Product],
  templateUrl: './products.html',
  styleUrl: './products.css'
})
export class Products implements OnInit {
  products: ProductModel[] = [];

  constructor(
    private productService: ProductService,
    private cartService: CartService
  ) {}

  ngOnInit(): void {
    this.productService.getAllProducts().subscribe({
      next: (data: any) => {
        this.products = data.products as ProductModel[];
      }
    });
  }

  onAddToCart(product: ProductModel) {
    this.cartService.addToCart(product);
  }
}