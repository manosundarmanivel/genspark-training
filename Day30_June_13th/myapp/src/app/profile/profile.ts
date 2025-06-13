import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserModel } from '../models/user';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './profile.html'
})
export class ProfileComponent implements OnInit {
  user: UserModel | null = null;

  ngOnInit(): void {
    // const storedUser = localStorage.getItem('loggedInUser');
    const storedUser = sessionStorage.getItem('loggedInUser');
    if (storedUser) {
      this.user = JSON.parse(storedUser); 
    }
  }
}
