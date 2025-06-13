import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthService {
private isLoggedInSubject = new BehaviorSubject<boolean>(false);
isLoggedIn$: Observable<boolean> = this.isLoggedInSubject.asObservable();

login(username: string, password: string): boolean {
if (username === 'admin' && password === 'admin') {
this.isLoggedInSubject.next(true);
return true;
}
return false;
}

logout(): void {
this.isLoggedInSubject.next(false);
}
}