
import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  FormGroup,
  Validators,
  AbstractControl,
  ValidationErrors,
  ReactiveFormsModule,
  FormControl
} from '@angular/forms';
import { BehaviorSubject, combineLatest } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';

export interface User {
  id: number;
  username: string;
  email: string;
  role: 'Admin' | 'User' | 'Guest';
}

@Component({
  selector: 'app-add-user',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './add-user.html',
  styleUrls: ['./add-user.css'],
})
export class AddUserComponent implements OnInit {
  form!: FormGroup;

  private users$ = new BehaviorSubject<User[]>([]);
  search$ = new BehaviorSubject<string>('');
  roleFilter$ = new BehaviorSubject<string>('All');

  searchControl = new FormControl('');

  filteredUsers$ = combineLatest([
    this.users$,
    this.search$,
    this.roleFilter$
  ]).pipe(
    map(([users, search, role]) =>
      users.filter(user =>
        (role === 'All' || user.role === role) &&
        (
          user.username.toLowerCase().includes(search.toLowerCase()) ||
          user.role.toLowerCase().includes(search.toLowerCase())
        )
      )
    )
  );


  constructor(private fb: FormBuilder) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      username: ['', [Validators.required, this.bannedUsernameValidator]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6), this.strongPasswordValidator]],
      confirmPassword: ['', Validators.required],
      role: ['User', Validators.required]
    }, { validators: this.passwordMatchValidator });

   this.searchControl.valueChanges.pipe(
    debounceTime(300),
    distinctUntilChanged()
  ).subscribe(value => this.search$.next(value ?? ''));
  }

  addUser(): void {
    if (this.form.invalid) return;

    const { confirmPassword, password, ...data } = this.form.value;

    const newUser: User = {
      id: Date.now(),
      username: data.username,
      email: data.email,
      role: data.role
    };

    this.users$.next([...this.users$.getValue(), newUser]);
    this.form.reset({ role: 'User' });
    alert('User Added!');
  }

  updateRole(value: string): void {
    this.roleFilter$.next(value);
  }

  onRoleChange(event: Event): void {
  const select = event.target as HTMLSelectElement;
  const value = select.value;
  this.updateRole(value);
}


  bannedUsernameValidator(control: AbstractControl): ValidationErrors | null {
    const banned = ['admin', 'root'];
    return banned.some(word => control.value?.toLowerCase().includes(word))
      ? { bannedName: true }
      : null;
  }

  strongPasswordValidator(control: AbstractControl): ValidationErrors | null {
    const val = control.value || '';
    const hasNumber = /\d/.test(val);
    const hasSymbol = /[!@#$%^&*]/.test(val);
    return hasNumber && hasSymbol ? null : { weakPassword: true };
  }

  passwordMatchValidator(group: AbstractControl): ValidationErrors | null {
    const password = group.get('password')?.value;
    const confirm = group.get('confirmPassword')?.value;
    return password === confirm ? null : { passwordMismatch: true };
  }
}