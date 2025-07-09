import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../auth.service';
import { CommonModule } from '@angular/common';
import { BehaviorSubject } from 'rxjs';

@Component({
  selector: 'app-login',
  standalone: true,
  templateUrl: './login.component.html',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule
  ]
})
export class LoginComponent {
  form: FormGroup;
  error = '';
  private loadingSubject = new BehaviorSubject<boolean>(false);
  loading$ = this.loadingSubject.asObservable();

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.form = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required]
    });
  }

  login() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loadingSubject.next(true);

    this.authService.login({
      username: this.form.value.email,
      password: this.form.value.password
    }).subscribe({
      next: () => {
        const role = this.authService.getRole()?.toLowerCase();
        if (role === 'admin') {
          this.router.navigate(['/admin-dashboard']);
        } else if (role === 'instructor') {
          this.router.navigate(['/instructor-dashboard']);
        } else {
          this.router.navigate(['/student-dashboard']);
        }
      },
      error: err => {
        this.error = err.error?.message || 'Login failed';
        this.loadingSubject.next(false);
      },
      complete: () => {
        this.loadingSubject.next(false);
      }
    });
  }
}
