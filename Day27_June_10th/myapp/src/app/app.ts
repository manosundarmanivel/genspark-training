import { Component } from '@angular/core';
import { First } from "./first/first";
import { Customer } from "./customer/customer";
import { ProductList } from "./product-list/product-list";
import { CommonModule } from '@angular/common';
import { CartService } from './cart.service';



@Component({
  selector: 'app-root',

  templateUrl: './app.html',
  styleUrl: './app.css',
  imports: [ CommonModule,Customer, ProductList,],
  
})


export class App {
  protected title = 'myapp';
cartCount = 0;  

  constructor(private cartService: CartService) {
    this.cartService.cartCount$.subscribe(count => {
      this.cartCount = count;
    });
  }
}
