import { ComponentFixture, TestBed } from '@angular/core/testing';
import { EnrolledCoursesComponent } from './enrolled-courses.component';
import { StudentService } from '../services/student.service';
import { of, throwError } from 'rxjs';
import { By } from '@angular/platform-browser';
import { RouterTestingModule } from '@angular/router/testing';
import { Component } from '@angular/core';

describe('EnrolledCoursesComponent', () => {
  let component: EnrolledCoursesComponent;
  let fixture: ComponentFixture<EnrolledCoursesComponent>;
  let mockStudentService: jasmine.SpyObj<StudentService>;

  const mockCourses = [
    {
      id: '1',
      title: 'Angular Deep Dive',
      description: 'Advanced Angular topics',
      thumbnailUrl: '',
      createdAt: new Date('2024-01-01T00:00:00Z'),
    },
  ];

  beforeEach(async () => {
    mockStudentService = jasmine.createSpyObj('StudentService', ['getEnrolledCourses']);

    await TestBed.configureTestingModule({
      imports: [RouterTestingModule, EnrolledCoursesComponent],
      providers: [{ provide: StudentService, useValue: mockStudentService }]
    }).compileComponents();
  });

  it('should create the component', () => {
    mockStudentService.getEnrolledCourses.and.returnValue(of([]));
    fixture = TestBed.createComponent(EnrolledCoursesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    expect(component).toBeTruthy();
  });

  it('should show enrolled course cards when courses are available', async () => {
    mockStudentService.getEnrolledCourses.and.returnValue(of(mockCourses));
    fixture = TestBed.createComponent(EnrolledCoursesComponent);
    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();

    const cards = fixture.debugElement.queryAll(By.css('.bg-white'));
    expect(cards.length).toBe(1);
    expect(cards[0].nativeElement.textContent).toContain('Angular Deep Dive');
  });

  it('should show fallback thumbnail if thumbnailUrl is empty', async () => {
    mockStudentService.getEnrolledCourses.and.returnValue(of(mockCourses));
    fixture = TestBed.createComponent(EnrolledCoursesComponent);
    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();

    const img = fixture.debugElement.query(By.css('img'));
    expect(img.nativeElement.src).toContain('https://static.skillshare.com/uploads/video/thumbnails');
  });

  it('should show error message when service fails', async () => {
    mockStudentService.getEnrolledCourses.and.returnValue(throwError(() => new Error('Server error')));
    fixture = TestBed.createComponent(EnrolledCoursesComponent);
    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();

    const errorDiv = fixture.debugElement.query(By.css('.text-red-600'));
    expect(errorDiv.nativeElement.textContent).toContain('Server error');
  });

  it('should show "no courses found" message if empty list is returned', async () => {
    mockStudentService.getEnrolledCourses.and.returnValue(of([]));
    fixture = TestBed.createComponent(EnrolledCoursesComponent);
    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();

    const noCoursesText = fixture.debugElement.query(By.css('.text-lg'));
    expect(noCoursesText.nativeElement.textContent).toContain('You haven’t enrolled in any courses yet.');
  });

  it('should have working routerLink for Browse Courses', async () => {
    mockStudentService.getEnrolledCourses.and.returnValue(of([]));
    fixture = TestBed.createComponent(EnrolledCoursesComponent);
    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();

    const browseLink = fixture.debugElement.query(By.css('a[routerLink="/courses"]'));
    expect(browseLink).toBeTruthy();
  });

  it('should have working routerLink for Become an Instructor', async () => {
    mockStudentService.getEnrolledCourses.and.returnValue(of([]));
    fixture = TestBed.createComponent(EnrolledCoursesComponent);
    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();

    const instructorLink = fixture.debugElement.query(By.css('a[routerLink="/instructor-dashboard/create-course"]'));
    expect(instructorLink).toBeTruthy();
  });
});
