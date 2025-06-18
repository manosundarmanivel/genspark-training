import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ProductService } from '../service/product.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-product-details',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './product-details.html',
  styleUrl: './product-details.css'
})
export class ProductDetails implements OnInit, OnDestroy {
  private route = inject(ActivatedRoute);
  private productService = inject(ProductService);

  product: any = null;
  error: string = '';
  isLoading = true;
  selectedImage: string | undefined;
  private routeSub!: Subscription;

  ngOnInit(): void {
    // Subscribe to paramMap for reactive updates
    this.routeSub = this.route.paramMap.subscribe(params => {
      const id = Number(params.get('id'));

      if (!isNaN(id)) {
        this.loadProduct(id);
      } else {
        this.error = 'Invalid product ID.';
        this.product = null;
        this.isLoading = false;
      }
    });
  }

  loadProduct(id: number): void {
    this.isLoading = true;
    this.error = '';
    this.product = null;

    this.productService.getProductDetails(id).subscribe({
      next: (data) => {
        this.product = data;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error fetching product details:', err);
        this.error = 'Unable to load product details.';
        this.isLoading = false;
      }
    });
  }

  ngOnDestroy(): void {
    if (this.routeSub) this.routeSub.unsubscribe();
  }
}
