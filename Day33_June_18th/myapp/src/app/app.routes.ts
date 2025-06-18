import { Routes } from '@angular/router';
import { Products } from './products/products';
import { Login } from './login/login';
import { About } from './about/about';
import { Profile } from './profile/profile';
import { ProductDetails } from './product-details/product-details';
import { authGuard } from './auth-guard';
import { Dashboard } from './dashboard/dashboard';
import { AddUser } from './add-user/add-user';

export const routes: Routes = [
  { path: '', redirectTo: 'home/guest', pathMatch: 'full' },  

  {
    path: 'home/:username',
    component: Products,
    canActivate: [authGuard],
    children: [
      {
        path: 'product/:id',
        component: ProductDetails,
        canActivate: [authGuard]
      }
    ]
  },

  {
    path: 'about',
    component: About,
   
  },

  { path: 'login', component: Login },
  { path: 'profile', component: Profile, canActivate: [authGuard] },
  {
    path:'dashboard', component: Dashboard
  },
  {
    path:'add-user', component: AddUser
  }

];
