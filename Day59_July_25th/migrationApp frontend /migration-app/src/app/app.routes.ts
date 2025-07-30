
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { Main } from './layout/main/main';
import { HomeComponent } from './pages/home/home';
import {  NewsComponent} from './pages/news/news';
import { ProductComponent } from './pages/products/products';
import {  ContactUsComponent } from './pages/contact/contact';
import { ColorComponent } from './pages/color/color';

import { CartComponent } from './pages/cart/cart';
import { OrderListComponent } from './pages/order/order';
import { CategoryComponent } from './pages/category/category';
import { NewsListComponent } from './pages/news-list/news-list';

export const routes: Routes = [
  {
    path: '',
    component: Main,
    children: [
      { path: 'home', component: HomeComponent },
      { path: 'news', component:  NewsComponent },
      { path: 'news-list', component:  NewsListComponent },
      { path: 'products', component:ProductComponent},
      { path: 'contact', component: ContactUsComponent },
      { path: 'color', component: ColorComponent },
     
      { path: 'cart', component: CartComponent },
      { path: 'order', component: OrderListComponent },
      {path :'category', component: CategoryComponent},
      { path: '', redirectTo: 'home', pathMatch: 'full' }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
