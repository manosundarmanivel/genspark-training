import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface UserProfile {
  id: string;
  username: string;
  role: string;
  fullName: string | null;
  phoneNumber: string | null;
  profilePictureUrl: string | null;
  bio: string | null;
}

@Injectable({ providedIn: 'root' })
export class ProfileService {
  private apiUrl = 'http://localhost:5295/api/v1/auth/profile';

  constructor(private http: HttpClient) {}

  getProfile(): Observable<{ success: boolean; data: UserProfile }> {
    return this.http.get<{ success: boolean; data: UserProfile }>(this.apiUrl);
  }

  updateProfile(profile: Partial<UserProfile>): Observable<{ success: boolean; message: string }> {
    return this.http.put<{ success: boolean; message: string }>(this.apiUrl, profile);
  }
}
