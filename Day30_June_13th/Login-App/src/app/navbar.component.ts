import { Component, OnInit } from '@angular/core';
import { AuthService } from './auth.service';

@Component({
  selector: 'app-navbar',
  template: `
    <h2>Navbar</h2>
    <p *ngIf="isLoggedIn">Welcome, admin! <button (click)="logout()">Logout</button></p>
    <p *ngIf="!isLoggedIn">Please log in.</p>
  `
})
export class NavbarComponent implements OnInit {
  isLoggedIn = false;

  constructor(private authService: AuthService) {}

  ngOnInit() {
    this.authService.isLoggedIn$.subscribe(status => {
      this.isLoggedIn = status;
    });
  }

  logout() {
    this.authService.logout();
  }
}