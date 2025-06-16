import { Component } from '@angular/core';

import { CommonModule } from '@angular/common';
import { Product } from "./product/product";
import { Products } from "./products/products";
import { Recipes } from "./recipes/recipes";
import { Cart } from "./cart/cart";
import { Login } from "./login/login";
import { ProfileComponent } from "./profile/profile";
import { RouterModule } from '@angular/router';




@Component({
  selector: 'app-root',

  templateUrl: './app.html',
  styleUrl: './app.css',
  imports: [Product, Products, Recipes, Cart, Login, ProfileComponent, Login, RouterModule],
  standalone: true
})


export class App {
  protected title = 'myapp';


  
}
