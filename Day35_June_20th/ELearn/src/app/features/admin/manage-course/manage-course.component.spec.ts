import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { ManageCourseComponent } from './manage-course.component';
import { AdminService } from '../services/admin.service';
import { of } from 'rxjs';
import { RouterTestingModule } from '@angular/router/testing';
import { By } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

describe('ManageCourseComponent', () => {
  let component: ManageCourseComponent;
  let fixture: ComponentFixture<ManageCourseComponent>;
  let mockAdminService: jasmine.SpyObj<AdminService>;

  const mockCourses = [
    { id: '1', title: 'Angular Basics', isDeleted: false },
    { id: '2', title: 'React Essentials', isDeleted: true },
    { id: '3', title: 'Vue Mastery', isDeleted: false }
  ];

  const mockUsers = [
    { id: 'u1', name: 'Alice', isDeleted: false },
    { id: 'u2', name: 'Bob', isDeleted: true }
  ];

  beforeEach(async () => {
    mockAdminService = jasmine.createSpyObj('AdminService', [
      'getAllCourses',
      'getAllUsers',
      'enableCourse',
      'disableCourse',
      'enableUser',
      'disableUser'
    ]);

    await TestBed.configureTestingModule({
      imports: [ManageCourseComponent, RouterTestingModule, FormsModule, CommonModule],
      providers: [
        { provide: AdminService, useValue: mockAdminService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ManageCourseComponent);
    component = fixture.componentInstance;

    mockAdminService.getAllCourses.and.returnValue(of({ data: mockCourses }));
    mockAdminService.getAllUsers.and.returnValue(of({ data: mockUsers }));
    mockAdminService.disableCourse.and.returnValue(of({}));
    mockAdminService.enableCourse.and.returnValue(of({}));

    fixture.detectChanges(); // triggers ngOnInit
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should load and display all courses', fakeAsync(() => {
    fixture.detectChanges();
    tick();

    component.filteredCourses$.subscribe(result => {
      expect(result.data.length).toBe(3);
    });
  }));

  it('should filter courses by search term', fakeAsync(() => {
    component.onSearchChange('Angular');
    tick();

    component.filteredCourses$.subscribe(result => {
      expect(result.data.length).toBe(1);
      expect(result.data[0].title).toContain('Angular');
    });
  }));




  it('should render course titles in the table', fakeAsync(() => {
    fixture.detectChanges();
    tick();
    fixture.detectChanges();

    const rows = fixture.debugElement.queryAll(By.css('tbody tr'));
    expect(rows.length).toBe(3);
    expect(rows[0].nativeElement.textContent).toContain('Angular Basics');
    expect(rows[1].nativeElement.textContent).toContain('React Essentials');
  }));
});
