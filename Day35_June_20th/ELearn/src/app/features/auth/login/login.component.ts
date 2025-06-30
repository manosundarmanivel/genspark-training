import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../auth.service';
import { CommonModule } from '@angular/common';

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
    this.form.markAllAsTouched(); // shows all errors immediately
    return;
  }

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
    error: err => this.error = err.error?.message || 'Login failed'
  });
}

}
