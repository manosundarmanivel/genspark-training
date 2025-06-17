import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../service/auth.service';
import { ProfileModel } from '../models/profile';
import { Router } from '@angular/router';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './profile.html',
  styleUrls: ['./profile.css'],
})
export class Profile implements OnInit {
  user: ProfileModel | null = null;
  error: string = '';

  constructor(private authService: AuthService,  private router: Router) {}

  ngOnInit(): void {
    this.authService.getProfile().subscribe({
      next: (data) => {
        this.user = new ProfileModel(data);
      },
      error: (err) => {
        console.error('Error fetching profile:', err);
        this.error = 'Failed to load profile.';
      }
    });
  }

    logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

}
