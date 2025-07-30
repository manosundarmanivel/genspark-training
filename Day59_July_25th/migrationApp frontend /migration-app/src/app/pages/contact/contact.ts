import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-contact-us',
  templateUrl: './contact.html',
  styleUrls: ['./contact.css'],
  imports : [CommonModule, ReactiveFormsModule]
})
export class ContactUsComponent {
  contactForm: FormGroup;
  successMessage = '';
  errorMessage = '';

  constructor(private fb: FormBuilder, private http: HttpClient) {
    this.contactForm = this.fb.group({
      name: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phone: [''],
      content: ['', Validators.required]
    });
  }

  onSubmit() {
    if (this.contactForm.invalid) return;

    const formData = this.contactForm.value;

    this.http.post('http://localhost:5228/api/ContactUs', formData).subscribe({
      next: () => {
        this.successMessage = 'Your message has been sent successfully.';
        this.errorMessage = '';
        this.contactForm.reset();
      },
      error: () => {
        this.errorMessage = 'Something went wrong. Please try again later.';
        this.successMessage = '';
      }
    });
  }
}
