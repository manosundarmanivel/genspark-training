import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { MyCoursesComponent } from './my-courses.component';
import { InstructorService } from '../services/instructor.service';
import { of, throwError } from 'rxjs';
import { By } from '@angular/platform-browser';
import { RouterTestingModule } from '@angular/router/testing';
import { CommonModule } from '@angular/common';

describe('MyCoursesComponent (standalone)', () => {
  let component: MyCoursesComponent;
  let fixture: ComponentFixture<MyCoursesComponent>;
  let mockInstructorService: any;

  const mockCourses = [
    {
      id: 'c1',
      title: 'Angular Mastery',
      description: 'Deep dive into Angular',
      instructorId: 'i1',
      thumbnailUrl: '',
      uploadedFiles: [
        { id: 'f1', fileName: 'file1.mp4', topic: 'Intro', description: 'Intro video' }
      ],
      enrolledStudents: [
        {
          fullName: 'John Doe',
          username: 'john@example.com',
          profilePictureUrl: '/images/john.jpg'
        }
      ]
    }
  ];

  beforeEach(async () => {
    mockInstructorService = {
      getInstructorCourses: jasmine.createSpy()
    };

    await TestBed.configureTestingModule({
      imports: [
        CommonModule,
        RouterTestingModule,
        MyCoursesComponent // standalone component directly imported
      ],
      providers: [
        { provide: InstructorService, useValue: mockInstructorService }
      ]
    }).compileComponents();
  });



  it('should show error on service failure', fakeAsync(() => {
    mockInstructorService.getInstructorCourses.and.returnValue(
      throwError(() => new Error('Service error'))
    );
    fixture = TestBed.createComponent(MyCoursesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    tick();
    fixture.detectChanges();

    const error = fixture.debugElement.query(By.css('.text-red-600')).nativeElement;
    expect(error.textContent).toContain('Service error');
  }));

  it('should show fallback UI for empty course list', fakeAsync(() => {
    mockInstructorService.getInstructorCourses.and.returnValue(of([]));
    fixture = TestBed.createComponent(MyCoursesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    tick();
    fixture.detectChanges();

    const fallback = fixture.debugElement.query(By.css('.text-center')).nativeElement;
    expect(fallback.textContent).toContain('You haven’t added any courses yet');
  }));

  it('should open and close modal correctly', fakeAsync(() => {
    mockInstructorService.getInstructorCourses.and.returnValue(of(mockCourses));
    fixture = TestBed.createComponent(MyCoursesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    tick();
    fixture.detectChanges();

    component.openModal(mockCourses[0]);
    fixture.detectChanges();

    const modal = fixture.debugElement.query(By.css('.fixed.inset-0'));
    expect(modal).toBeTruthy();

    component.closeModal();
    fixture.detectChanges();

    const closedModal = fixture.debugElement.query(By.css('.fixed.inset-0'));
    expect(closedModal).toBeNull();
  }));
});
