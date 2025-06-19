import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { UserModel } from '../models/user';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private loginUrl = 'https://dummyjson.com/auth/login';

  constructor(private http: HttpClient) {}

  login(user: UserModel): Observable<any> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    const body = {
      username: user.username,
      password: user.password,
      expiresInMins: 30
    };

    return this.http.post(this.loginUrl, body, {
      headers,
    
    });
  }

  getProfile(): Observable<any> {
    const accessToken = localStorage.getItem('accessToken');
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${accessToken}`
    });

    return this.http.get('https://dummyjson.com/auth/me', { headers });
  }

  logout(): void {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('loggedInUser');
  }
}
