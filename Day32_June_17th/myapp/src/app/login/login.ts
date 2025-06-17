import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../service/auth.service';
import { UserModel } from '../models/user';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.html'
})
export class Login {
  user = new UserModel();
  message = '';
  isLoading = false;

  constructor(private authService: AuthService, private router: Router) {}

  onLogin() {
    this.isLoading = true;
    this.authService.login(this.user).subscribe({
      next: (res) => {
        this.isLoading = false;
        this.message = 'Login successful!';

       
        localStorage.setItem('accessToken', res.accessToken);
        localStorage.setItem('loggedInUser', JSON.stringify(res));

  
        this.router.navigate(['/home', this.user.username]);
      },
      error: (err) => {
        this.isLoading = false;
        this.message = 'Invalid username or password.';
        console.error(err);
      }
    });
  }
}
