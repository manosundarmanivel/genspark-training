import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Component } from '@angular/core';

import { Product } from './product';
import { ProductModel } from '../models/product';

@Component({
  standalone: true,
  imports: [Product],
  template: `
    <app-product
      [product]="product"
      (addToCart)="handleAddToCart($event)">
    </app-product>
  `
})
class HostComponent {
  product: ProductModel = {
    id: 1,
    title: 'Abc',
    description: 'Some description',
    price: 100
  } as ProductModel;

  addedProduct: ProductModel | null = null;

  handleAddToCart(product: ProductModel) {
    this.addedProduct = product;
  }
}

fdescribe('Product Component', () => {
  let fixture: ComponentFixture<HostComponent>;
  let hostComponent: HostComponent;
  let compiled: HTMLElement;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HostComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(HostComponent);
    hostComponent = fixture.componentInstance;
    fixture.detectChanges();
    compiled = fixture.nativeElement as HTMLElement;
  });

  it('should render the product title and description', () => {
   expect(compiled.textContent).toContain('Quick View'); // or 'blah blah'

    expect(compiled.textContent).toContain('Some description');
  });

  it('should emit addToCart event when method is triggered', () => {
    const productComponent: Product = fixture.debugElement.children[0].componentInstance;
    productComponent.onAddToCart();

    expect(hostComponent.addedProduct?.id).toBe(1);
  });
});
