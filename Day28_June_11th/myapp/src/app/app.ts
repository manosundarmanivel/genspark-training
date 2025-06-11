import { Component } from '@angular/core';

import { CommonModule } from '@angular/common';
import { Product } from "./product/product";
import { Products } from "./products/products";
import { Recipes } from "./recipes/recipes";




@Component({
  selector: 'app-root',

  templateUrl: './app.html',
  styleUrl: './app.css',
  imports: [Product, Products, Recipes],
  
})


export class App {
  protected title = 'myapp';


  
}
