import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-register',
  standalone: true,
  templateUrl: './register.component.html',
  imports: [
    ReactiveFormsModule, CommonModule, RouterModule
  ]
})
export class RegisterComponent {
  form: FormGroup;
  error = '';


  roles = [
    { id: 1, name: 'Student' },
    { id: 2, name: 'Instructor' },
    // { id: 3, name: 'Admin' }
  ];

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.form = this.fb.group({
      username: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
      roleId: [2, Validators.required] 
    });
  }

 register() {
  if (this.form.invalid) return;

  const selectedRole = this.roles.find(r => r.id === this.form.value.roleId);

  if (!selectedRole) {
    this.error = 'Please select a valid role.';
    return;
  }

  const payload = {
    username: this.form.value.username,
    password: this.form.value.password,
    role: selectedRole.name  // this must be "Student", "Instructor", or "Admin"
  };

  this.authService.register(payload).subscribe({
    next: () => this.router.navigate(['/login']),
    error: err => this.error = err.error?.message || 'Registration failed'
  });
}

}
