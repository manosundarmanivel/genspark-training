import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

interface User {
  id?: number;
  firstName: string;
  lastName: string;
  gender: string;
  role: string;
  state: string;
}

@Component({
  selector: 'app-add-user',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './add-user.html',
  styleUrls: ['./add-user.css']
})
export class AddUser implements OnInit {

  newUser: User = {
    firstName: '',
    lastName: '',
    gender: '',
    role: '',
    state: ''
  };

  users: User[] = [];

  ngOnInit(): void {
  }

  addUser() {
    fetch('https://dummyjson.com/users/add', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(this.newUser)
    })
    .then(res => res.json())
    .then(user => {
      this.users.push(user);
      alert('User added successfully!');
      this.newUser = {
        firstName: '',
        lastName: '',
        gender: '',
        role: '',
        state: ''
      };
    })
    .catch(err => console.error('Error adding user:', err));
  }
}
