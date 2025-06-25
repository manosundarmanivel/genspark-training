import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize } from 'rxjs';
import { ProfileService, UserProfile } from '../../services/profile.service';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './profile.component.html',
})
export class ProfileComponent implements OnInit {
  private fb = inject(FormBuilder);
  private profileService = inject(ProfileService);
  private cdr = inject(ChangeDetectorRef);

  profileForm!: FormGroup;
  isEditing = false;
  isLoading = false;

  successMessage = '';
  errorMessage = '';
  previewUrl: string | ArrayBuffer | null = null;
  selectedFile: File | null = null;

  username = '';
  role = '';
  originalProfileData: UserProfile | null = null;

  ngOnInit(): void {
    this.initializeForm();
    this.loadProfile();
  }

  initializeForm(): void {
    this.profileForm = this.fb.group({
      fullName: ['', Validators.required],
      phoneNumber: [''],
      profilePictureUrl: [''],
      bio: ['']
    });
    this.profileForm.disable();
  }

  loadProfile(): void {
    this.isLoading = true;
    this.profileService.getProfile()
      .pipe(finalize(() => {
        this.isLoading = false;
        this.cdr.detectChanges(); // Ensure UI sync
      }))
      .subscribe({
        next: (res) => {
          if (!res || !res.data) {
            this.errorMessage = 'Invalid profile data received.';
            return;
          }

          const profile = res.data as UserProfile;
          this.originalProfileData = { ...profile };

          this.username = profile.username || '';
          this.role = profile.role || '';

          this.profileForm.patchValue({
            fullName: profile.fullName || '',
            phoneNumber: profile.phoneNumber || '',
            profilePictureUrl: profile.profilePictureUrl || '',
            bio: profile.bio || ''
          });

          this.previewUrl = profile.profilePictureUrl;
        },
        error: (err) => {
          console.error(err);
          this.errorMessage = 'Failed to load profile';
        }
      });
  }

  onProfilePicChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (!input.files?.length) return;

    this.selectedFile = input.files[0];

    const reader = new FileReader();
    reader.onload = () => {
      this.previewUrl = reader.result;
      this.profileForm.patchValue({ profilePictureUrl: '' }); // clear old URL
      this.cdr.detectChanges();
    };
    reader.readAsDataURL(this.selectedFile);
  }

  enableEdit(): void {
    this.isEditing = true;
    this.successMessage = '';
    this.errorMessage = '';
    this.profileForm.enable();
  }

  cancelEdit(): void {
    if (this.originalProfileData) {
      this.profileForm.patchValue({
        fullName: this.originalProfileData.fullName || '',
        phoneNumber: this.originalProfileData.phoneNumber || '',
        profilePictureUrl: this.originalProfileData.profilePictureUrl || '',
        bio: this.originalProfileData.bio || ''
      });
      this.previewUrl = this.originalProfileData.profilePictureUrl;
    }
    this.selectedFile = null;
    this.isEditing = false;
    this.profileForm.disable();
  }

  updateProfile(): void {
    const formValue = this.profileForm.getRawValue();
    const formData = new FormData();

    formData.append('fullName', formValue.fullName);
    formData.append('phoneNumber', formValue.phoneNumber || '');
    formData.append('bio', formValue.bio || '');

    if (this.selectedFile) {
      formData.append('profilePictureUrl', this.selectedFile);
    }

    this.isLoading = true;
    this.profileService.updateProfile(formData)
      .pipe(finalize(() => {
        this.isLoading = false;
        this.cdr.detectChanges();
      }))
      .subscribe({
        next: (res) => {
          this.successMessage = res.message;
          this.isEditing = false;
          this.profileForm.disable();
          this.selectedFile = null;
          this.loadProfile(); // reload to sync username/role
        },
        error: () => {
          this.errorMessage = 'Profile update failed';
        }
      });
  }
}
