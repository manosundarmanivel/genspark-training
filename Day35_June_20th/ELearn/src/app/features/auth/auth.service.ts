import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs/operators';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private baseUrl = 'http://localhost:5295/api/v1/auth';

  constructor(private http: HttpClient, private router: Router) {}

  login(data: { username: string; password: string }) {
    return this.http.post(`${this.baseUrl}/login`, data).pipe(
      tap((res: any) => {
        const token = res.token;
        localStorage.setItem('token', token);
        localStorage.setItem('refreshToken', res.refreshToken);

    
        const payload = JSON.parse(atob(token.split('.')[1]));
        const role = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
        localStorage.setItem('role', role); 
      })
    );
  }

  getRole(): string | null {
    return localStorage.getItem('role');
  }

  refreshToken(refreshToken: string): Observable<any> {
    return this.http.post('http://localhost:5295/api/v1/auth/refresh', {
      refreshToken
    });
  }

  register(data: any) {
    return this.http.post(`${this.baseUrl}/register`, data);
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('role');
    this.router.navigate(['/login']);
  }

  getToken() {
    return localStorage.getItem('token');
  }

   getRefreshToken() {
    return localStorage.getItem('refreshToken');
  }

  
  storeToken(token: string, refreshToken: string) {
    localStorage.setItem('token', token);
    localStorage.setItem('refreshToken', refreshToken);
  }


  isLoggedIn() {
    return !!this.getToken();
  }
}
