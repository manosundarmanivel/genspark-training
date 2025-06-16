import {
  Component,
  OnInit,
  inject,
  HostListener,
  OnDestroy,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  debounceTime,
  distinctUntilChanged,
  switchMap,
  tap,
} from 'rxjs/operators';
import { Subject, Subscription, of } from 'rxjs';
import { ProductModel } from '../models/product';
import { ProductService } from '../service/product.service';
import { CartService } from '../service/cart.service';
import { Product } from '../product/product';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [CommonModule, FormsModule, Product],
  templateUrl: './products.html',
  styleUrl: './products.css',
})

export class Products implements OnInit, OnDestroy {
  products: ProductModel[] = [];
  searchInput: string = '';
  isLoading = false;
  skip = 0;
  hasMore = true;
  showBackToTop = false;

  private searchSubject = new Subject<string>();
  private searchSubscription!: Subscription;

  private productService = inject(ProductService);
  private cartService = inject(CartService);

  ngOnInit(): void {
    this.searchSubscription = this.searchSubject
      .pipe(
        debounceTime(400),
        distinctUntilChanged(),
        tap(() => {
          this.isLoading = true;
          this.skip = 0;
          this.hasMore = true;
        }),
        switchMap((term) =>
          this.productService.getSearchProducts(term, this.skip)
        ),
        tap(() => (this.isLoading = false))
      )
      .subscribe({
        next: (data) => {
          this.products = data.products;
        },
        error: (err) => {
          this.isLoading = false;
          console.error(err);
        },
      });

    this.searchSubject.next('');
  }

  onSearchChange(value: string) {
    this.searchSubject.next(value);
  }

  onAddToCart(product: ProductModel) {
    this.cartService.addToCart(product);
  }

  @HostListener('window:scroll', [])
  onScroll(): void {
    const scrollY = window.scrollY;
    this.showBackToTop = scrollY > 300;

    if (
      window.innerHeight + scrollY >= document.body.offsetHeight - 100 &&
      !this.isLoading &&
      this.hasMore
    ) {
      this.loadMore();
    }
  }

  scrollToTop(): void {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  loadMore(): void {
    this.isLoading = true;
    this.skip += 10;

    this.productService
      .getSearchProducts(this.searchInput, this.skip)
      .pipe(tap(() => (this.isLoading = false)))
      .subscribe({
        next: (data) => {
          if (data.products.length === 0) {
            this.hasMore = false;
          } else {
            this.products = [...this.products, ...data.products];
          }
        },
        error: (err) => {
          this.isLoading = false;
          console.error('Error during load more:', err);
        },
      });
  }

  highlight(text: string): string {
    if (!this.searchInput) return text;
    const pattern = new RegExp(`(${this.escapeRegExp(this.searchInput)})`, 'gi');
    return text.replace(pattern, `<mark>$1</mark>`);
  }

  escapeRegExp(text: string): string {
    return text.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
  }

  ngOnDestroy(): void {
    this.searchSubscription?.unsubscribe();
  }
}
