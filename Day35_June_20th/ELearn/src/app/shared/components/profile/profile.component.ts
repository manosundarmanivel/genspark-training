import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ProfileService, UserProfile } from '../../services/profile.service';
import { BehaviorSubject, catchError, map, of, switchMap, tap } from 'rxjs';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './profile.component.html',
})
export class ProfileComponent implements OnInit {
  private fb = inject(FormBuilder);
  private profileService = inject(ProfileService);

  profileForm!: FormGroup;
  isEditing = false;
  selectedFile: File | null = null;

  successMessage = '';
  errorMessage = '';

  previewUrl = signal<string | ArrayBuffer | null>(null);

  private profileSubject = new BehaviorSubject<UserProfile | null>(null);
  profile$ = this.profileSubject.asObservable();

  ngOnInit(): void {
    this.profileForm = this.fb.group({
    fullName: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(50)]],
    phoneNumber: [
      '',
      [
        Validators.pattern(/^\+?[0-9]{7,15}$/) 
      ]
    ],
    profilePictureUrl: [''],
    bio: ['', [Validators.maxLength(300)]]
  });
    this.profileForm.disable();
    this.loadProfile();
  }

  loadProfile(): void {
    this.profileService.getProfile().pipe(
      map(res => res.data as UserProfile),
      tap(profile => {
        this.profileSubject.next(profile); 
        this.profileForm.patchValue({
          fullName: profile.fullName,
          phoneNumber: profile.phoneNumber,
          profilePictureUrl: profile.profilePictureUrl,
          bio: profile.bio
        });
        this.previewUrl.set(profile.profilePictureUrl ?? null);
      }),
      catchError(err => {
        this.errorMessage = 'Failed to load profile';
        return of(null);
      })
    ).subscribe();
  }

  onProfilePicChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (!input.files?.length) return;

    this.selectedFile = input.files[0];

    const reader = new FileReader();
    reader.onload = () => {
      this.previewUrl.set(reader.result);
      this.profileForm.patchValue({ profilePictureUrl: '' });
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
    const profile = this.profileSubject.value;
    if (profile) {
      this.profileForm.patchValue({
        fullName: profile.fullName,
        phoneNumber: profile.phoneNumber,
        bio: profile.bio,
        profilePictureUrl: profile.profilePictureUrl
      });
      this.previewUrl.set(profile.profilePictureUrl ?? null);
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

    this.successMessage = '';
    this.errorMessage = '';

    this.profileService.updateProfile(formData).pipe(
      switchMap(() => this.profileService.getProfile()),
      tap(res => {
        const updatedProfile = res.data as UserProfile;
        this.profileSubject.next(updatedProfile); // 🔁 update observable value
        this.previewUrl.set(updatedProfile.profilePictureUrl ?? null); // ✅ update preview

        this.profileForm.patchValue({
          fullName: updatedProfile.fullName,
          phoneNumber: updatedProfile.phoneNumber,
          profilePictureUrl: updatedProfile.profilePictureUrl,
          bio: updatedProfile.bio
        });

        this.successMessage = 'Profile updated successfully';
        this.isEditing = false;
        this.profileForm.disable();
        this.selectedFile = null;
      }),
      catchError(() => {
        this.errorMessage = 'Profile update failed';
        return of(null);
      })
    ).subscribe();
  }
}
