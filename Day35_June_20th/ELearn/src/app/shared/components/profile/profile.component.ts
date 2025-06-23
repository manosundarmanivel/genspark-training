import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { ProfileService, UserProfile } from '../../services/profile.service';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './profile.component.html',
})
export class ProfileComponent implements OnInit {
  profileForm!: FormGroup;
  profile!: UserProfile;
  isEditing = false;
  isLoading = true;
  successMessage = '';
  errorMessage = '';

  constructor(private fb: FormBuilder, private profileService: ProfileService) {}

  ngOnInit(): void {
    this.loadProfile();
  }

  loadProfile(): void {
    this.profileService.getProfile().subscribe({
      next: (res) => {
        this.profile = res.data;
        this.profileForm = this.fb.group({
          fullName: [this.profile.fullName],
          phoneNumber: [this.profile.phoneNumber],
          profilePictureUrl: [this.profile.profilePictureUrl],
          bio: [this.profile.bio]
        });
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Failed to load profile';
        this.isLoading = false;
      }
    });
  }

  enableEdit() {
    this.isEditing = true;
    this.successMessage = '';
    this.errorMessage = '';
  }

  cancelEdit() {
    this.isEditing = false;
    if (this.profileForm && this.profile) {
      this.profileForm.patchValue({
        fullName: this.profile.fullName,
        phoneNumber: this.profile.phoneNumber,
        profilePictureUrl: this.profile.profilePictureUrl,
        bio: this.profile.bio
      });
    }
  }

  updateProfile(): void {
    if (this.profileForm.invalid) return;

    this.profileService.updateProfile(this.profileForm.value).subscribe({
      next: (res) => {
        this.successMessage = res.message;
        this.isEditing = false;
        this.loadProfile();
      },
      error: () => {
        this.errorMessage = 'Profile update failed';
      }
    });
  }
}
