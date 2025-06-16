import { Routes } from '@angular/router';
import { Products } from './products/products';
import { Login } from './login/login';
import { About } from './about/about';


export const routes: Routes = [
  { path: '', component: Products },      
  { path: 'home', component: Products },
  { path: 'about', component: About }
];
