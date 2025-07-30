import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProductService, ProductDto } from '../products/product.service';
import { CategoryService, Category } from '../category/category.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './home.html',
  styleUrls: ['./home.css']
})
export class HomeComponent implements OnInit {
  products: ProductDto[] = [];
  categories: Category[] = [];
  selectedCategoryId: number | null = null;

  constructor(
    private productService: ProductService,
    private categoryService: CategoryService
  ) {}

  ngOnInit(): void {
    this.loadCategories();
    this.loadProducts();
  }

  loadCategories(): void {
    this.categoryService.getAll().subscribe(data => {
      this.categories = data;
    });
  }

  loadProducts(): void {
    this.productService.getAll().subscribe(data => {
      this.products = data;
    });
  }

  get filteredProducts(): ProductDto[] {
    if (!this.selectedCategoryId) return this.products;
    return this.products.filter(p => p.categoryId === this.selectedCategoryId);
  }

  selectCategory(categoryId: number): void {
    this.selectedCategoryId = categoryId;
  }

  addToCart(product: ProductDto): void {
    const cart = JSON.parse(sessionStorage.getItem('cart') || '[]');
    const existing = cart.find((p: ProductDto) => p.productId === product.productId);
    if (!existing) {
      cart.push({ ...product, quantity: 1 });
    } else {
      existing.quantity += 1;
    }
    sessionStorage.setItem('cart', JSON.stringify(cart));
    alert('Product added to cart!');
  }
}
