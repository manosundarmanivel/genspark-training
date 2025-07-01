import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { ManageUserComponent } from './manage-user.component';
import { AdminService } from '../services/admin.service';
import { of } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

describe('ManageUserComponent', () => {
  let component: ManageUserComponent;
  let fixture: ComponentFixture<ManageUserComponent>;
  let mockAdminService: jasmine.SpyObj<AdminService>;

  const mockUsers = [
    { id: 'u1', username: 'john.doe', isDeleted: false, role: { name: 'Student' } },
    { id: 'u2', username: 'jane.admin', isDeleted: false, role: { name: 'Admin' } },
    { id: 'u3', username: 'disabled.user', isDeleted: true, role: { name: 'Instructor' } }
  ];

  beforeEach(async () => {
    mockAdminService = jasmine.createSpyObj('AdminService', [
      'getAllUsers', 'disableUser', 'enableUser'
    ]);

    await TestBed.configureTestingModule({
      imports: [CommonModule, FormsModule, ManageUserComponent],
      providers: [{ provide: AdminService, useValue: mockAdminService }]
    }).compileComponents();

    fixture = TestBed.createComponent(ManageUserComponent);
    component = fixture.componentInstance;
    mockAdminService.getAllUsers.and.returnValue(of({ data: mockUsers }));
    fixture.detectChanges();
  });

  it('should create the component and load users', fakeAsync(() => {
    component.ngOnInit();
    tick();

    component.users$.subscribe(result => {
      expect(result.data.length).toBe(3);
    });
  }));

  it('should filter users by search term', fakeAsync(() => {
    component.onSearchChange('john');
    tick();

    component.filteredUsers$.subscribe(result => {
      expect(result.data.length).toBe(1);
      expect(result.data[0].username).toContain('john');
    });
  }));

  it('should filter users by role', fakeAsync(() => {
    component.onRoleFilterChange('Admin');
    tick();

    component.filteredUsers$.subscribe(result => {
      expect(result.data.length).toBe(1);
      expect(result.data[0].role.name).toBe('Admin');
    });
  }));





  it('should toggle user status from enabled to disabled', fakeAsync(() => {
    mockAdminService.disableUser.and.returnValue(of({}));

    const user = { ...mockUsers[0] }; // john.doe (enabled)
    component['usersSubject'].next({ data: mockUsers }); // reset users
    component.toggleUserStatus(user);
    tick();

    const updated = component['usersSubject'].value.data.find(u => u.id === user.id);
    expect(updated?.isDeleted).toBeTrue();
  }));

  it('should toggle user status from disabled to enabled', fakeAsync(() => {
    mockAdminService.enableUser.and.returnValue(of({}));

    const user = { ...mockUsers[2] }; // disabled.user (disabled)
    component['usersSubject'].next({ data: mockUsers }); // reset users
    component.toggleUserStatus(user);
    tick();

    const updated = component['usersSubject'].value.data.find(u => u.id === user.id);
    expect(updated?.isDeleted).toBeFalse();
  }));
});
