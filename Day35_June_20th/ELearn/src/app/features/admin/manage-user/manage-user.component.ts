import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminService } from '../services/admin.service';
import { Observable, of, BehaviorSubject, combineLatest } from 'rxjs';
import { catchError, map, startWith } from 'rxjs/operators';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-manage-user',
  standalone: true,
  templateUrl: './manage-user.component.html',
  imports: [CommonModule, FormsModule],
})
export class ManageUserComponent implements OnInit {
  private usersSubject = new BehaviorSubject<{ data: any[] }>({ data: [] });
  users$ = this.usersSubject.asObservable();
  

  searchTerm = '';
  selectedRole = '';
  selectedStatus = '';
private selectedStatus$ = new BehaviorSubject<string>('');
  private searchTerm$ = new BehaviorSubject<string>('');
  private selectedRole$ = new BehaviorSubject<string>('');

filteredUsers$ = combineLatest([
  this.users$,
  this.searchTerm$.pipe(startWith('')),
  this.selectedRole$.pipe(startWith('')),
  this.selectedStatus$.pipe(startWith(''))
]).pipe(
  map(([wrapper, term, role, status]) => {
    const lowerTerm = term.toLowerCase().trim();
    return {
      data: wrapper.data.filter(user =>
        (!lowerTerm || user.username?.toLowerCase().includes(lowerTerm)) &&
        (!role || user.role?.name === role || user.role === role) &&
        (!status ||
          (status === 'enabled' && !user.isDeleted) ||
          (status === 'disabled' && user.isDeleted))
      )
    };
  })
);

onStatusFilterChange(status: string): void {
  this.selectedStatus$.next(status);
}

  constructor(private adminService: AdminService) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.adminService.getAllUsers().pipe(
      catchError(() => of({ data: [] }))
    ).subscribe(result => this.usersSubject.next(result));
  }

  toggleUserStatus(user: any): void {
    const action$ = user.isDeleted
      ? this.adminService.enableUser(user.id)
      : this.adminService.disableUser(user.id);

    action$.subscribe(() => {
      const users = this.usersSubject.value.data.map(u =>
        u.id === user.id ? { ...u, isDeleted: !u.isDeleted } : u
      );
      this.usersSubject.next({ data: users });
    });
  }

  onSearchChange(term: string): void {
    this.searchTerm$.next(term);
  }

  onRoleFilterChange(role: string): void {
    this.selectedRole$.next(role);
  }

  get uniqueRoles(): string[] {
    const allRoles = this.usersSubject.value.data.map(u => u.role?.name || u.role);
    return Array.from(new Set(allRoles.filter(Boolean)));
  }

  trackByUserId(index: number, user: any): string {
    return user.id;
  }
}
