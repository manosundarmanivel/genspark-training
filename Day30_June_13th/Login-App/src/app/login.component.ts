import { Component } from '@angular/core';
import { AuthService } from './auth.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-login',
  imports: [CommonModule, FormsModule],
  standalone: true,
  template: `
    <h3>Login</h3>
    <input [(ngModel)]="username" placeholder="Username" />
    <input [(ngModel)]="password" type="password" placeholder="Password" />
    <button (click)="onLogin()">Login</button>
    <p *ngIf="loginFailed" style="color:red;">Invalid credentials</p>
  `
})
export class LoginComponent {
  username = '';
  password = '';
  loginFailed = false;

  constructor(private authService: AuthService) {}

  onLogin() {
    const success = this.authService.login(this.username, this.password);
    this.loginFailed = !success;
  }
}