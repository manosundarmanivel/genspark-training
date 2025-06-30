import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { LoginComponent } from "./features/auth/login/login.component";
import { RegisterComponent } from "./features/auth/register/register.component";
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthInterceptor } from './features/auth/auth.interceptor';
import 'chart.js/auto';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, LoginComponent, RegisterComponent],
  templateUrl: './app.html',
  styleUrl: './app.css',


 
  
})
export class App {
  protected title = 'ELearn';
}
