import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../service/auth.service';
import { UserModel } from '../models/user';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.html'
})
export class Login {
  user = new UserModel();
  message = '';

  constructor(private authService: AuthService) {}

  onLogin() {
    const success = this.authService.login(this.user);
    if (success) {
      this.message = 'Login successful!';
      // localStorage.setItem('loggedInUser', JSON.stringify(this.user)); 
      sessionStorage.setItem('loggedInUser', JSON.stringify(this.user)); 
    } else {
      this.message = 'Invalid username or password.';
    }
  }
}
