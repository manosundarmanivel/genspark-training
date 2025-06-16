import { Injectable } from '@angular/core';
import { UserModel } from '../models/user';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private dummyUsers: UserModel[] = [
    new UserModel('admin', 'admin123'),
    new UserModel('user', 'user123')
  ];

  login(user: UserModel): boolean {
    return this.dummyUsers.some(
      u => u.username === user.username && u.password === user.password
    );
  }
}
