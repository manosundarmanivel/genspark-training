import { Component } from '@angular/core';

import { CommonModule } from '@angular/common';
import { Product } from "./product/product";
import { Products } from "./products/products";
import { Recipes } from "./recipes/recipes";
import { Cart } from "./cart/cart";
import { Login } from "./login/login";
import { Profile } from "./profile/profile";
import { RouterModule } from '@angular/router';
import { ProductDetails } from './product-details/product-details';




@Component({
  selector: 'app-root',

  templateUrl: './app.html',
  styleUrl: './app.css',
  imports: [Product, Products, Recipes, Cart, Login, Profile, Login, RouterModule, ProductDetails],
  standalone: true
})


export class App {
  protected title = 'myapp';

protected username = localStorage.getItem('loggedInUser') ? JSON.parse(localStorage.getItem('loggedInUser')!).username : 'Guest';


  
}
