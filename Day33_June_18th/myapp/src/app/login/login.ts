import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormsModule, Validators } from '@angular/forms';
import { AuthService } from '../service/auth.service';
import { UserModel } from '../models/user';
import { Router } from '@angular/router';
import { ReactiveFormsModule } from '@angular/forms';
import { textValidator } from '../validators/text-validator.validator';


@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './login.html',
  styleUrls: ['./login.css'],
})
// export class Login {
//   user = new UserModel();
//   message = '';
//   isLoading = false;

//   constructor(private authService: AuthService, private router: Router) {}

//   onLogin() {
//     this.isLoading = true;
//     this.authService.login(this.user).subscribe({
//       next: (res) => {
//         this.isLoading = false;
//         this.message = 'Login successful!';

       
//         localStorage.setItem('accessToken', res.accessToken);
//         localStorage.setItem('loggedInUser', JSON.stringify(res));

  
//         this.router.navigate(['/home', this.user.username]);
//       },
//       error: (err) => {
//         this.isLoading = false;
//         this.message = 'Invalid username or password.';
//         console.error(err);
//       }
//     });
//   }
// }


export class Login {
  loginForm: FormGroup;
  constructor(private authService: AuthService, private router: Router, private fb: FormBuilder) {
    this.loginForm = this.fb.group({
      un: ['', Validators.required],
      pass: ['', [Validators.required, textValidator()]]
    });
  }

  handleLogin() {
    if (this.loginForm.valid) {
      const formValue = this.loginForm.value;
      console.log('Login Success', formValue);
      this.authService.login(formValue).subscribe({
        next: (res) => {
          console.log('Login successful', res);
          localStorage.setItem('accessToken', res.accessToken);
          localStorage.setItem('loggedInUser', JSON.stringify(res));
          this.router.navigate(['/home', formValue.un]);
        },
        error: (err) => {
          console.error('Login failed', err);
          alert('Invalid username or password.');
        }
      });
    }
  }
}