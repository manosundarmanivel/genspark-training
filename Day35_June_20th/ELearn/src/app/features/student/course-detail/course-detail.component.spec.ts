import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { CourseDetailComponent } from './course-detail.component';
import { StudentService } from '../services/student.service';
import { ActivatedRoute, Router } from '@angular/router';
import { of, throwError, BehaviorSubject } from 'rxjs';
import { CommonModule } from '@angular/common';
import { By } from '@angular/platform-browser';
import { DomSanitizer } from '@angular/platform-browser';
import { ToastrService } from 'ngx-toastr';

describe('CourseDetailComponent', () => {
  let component: CourseDetailComponent;
  let fixture: ComponentFixture<CourseDetailComponent>;
  let studentServiceSpy: jasmine.SpyObj<StudentService>;
  let routerSpy: jasmine.SpyObj<Router>;
  let sanitizer: DomSanitizer;
  let paramMapSubject: BehaviorSubject<any>;

  const mockCourse = {
    id: 'course1',
    title: 'Test Course',
    description: 'Test Description',
    instructorName: 'Test Instructor',
    domain: 'Web',
    language: 'English',
    level: 'Beginner',
    isEnrolled: true,
    isCompleted: false,
    uploadedFiles: [
      {
        id: 'file1',
        fileName: 'video1.mp4',
        topic: 'Intro',
        description: 'Intro desc',
        uploadedAt: new Date(),
        isCompleted: false,
        path: '/uploads/video1.mp4'
      },
      {
        id: 'file2',
        fileName: 'video2.mp4',
        topic: 'Advanced',
        description: 'Adv desc',
        uploadedAt: new Date(),
        isCompleted: false,
        path: '/uploads/video2.mp4'
      }
    ],
    firstUploadedFile: {
      id: 'file1',
      fileName: 'video1.mp4',
      topic: 'Intro',
      description: 'Intro desc',
      uploadedAt: new Date(),
      isCompleted: false,
      path: '/uploads/video1.mp4'
    }
  };

let toastrServiceSpy: jasmine.SpyObj<ToastrService>;

beforeEach(async () => {
  paramMapSubject = new BehaviorSubject({
    get: (key: string) => key === 'courseId' ? 'course1' : null
  });

  studentServiceSpy = jasmine.createSpyObj('StudentService', [
    'getCourseById',
    'enrollInCourse',
    'markFileAsCompleted'
  ]);
  studentServiceSpy.getCourseById.and.returnValue(of({ data: mockCourse }));
  studentServiceSpy.enrollInCourse.and.returnValue(of({}));
  studentServiceSpy.markFileAsCompleted.and.returnValue(of({}));

  routerSpy = jasmine.createSpyObj('Router', ['navigate']);

  // ✅ Proper toastr spy
  toastrServiceSpy = jasmine.createSpyObj('ToastrService', ['success', 'error']);

  await TestBed.configureTestingModule({
    imports: [CommonModule, CourseDetailComponent],
    providers: [
      { provide: StudentService, useValue: studentServiceSpy },
      { provide: ActivatedRoute, useValue: { paramMap: paramMapSubject.asObservable() } },
      { provide: Router, useValue: routerSpy },
      { provide: ToastrService, useValue: toastrServiceSpy }
    ]
  }).compileComponents();

  fixture = TestBed.createComponent(CourseDetailComponent);
  component = fixture.componentInstance;
  sanitizer = TestBed.inject(DomSanitizer);
  fixture.detectChanges();
});


  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should load and display course details', fakeAsync(() => {
    tick(); // flush observables
    fixture.detectChanges();

    // Should show course title and description
    const title = fixture.debugElement.query(By.css('h1'));
    expect(title.nativeElement.textContent).toContain('Test Course');
    const desc = fixture.debugElement.query(By.css('.text-gray-600'));
    expect(desc.nativeElement.textContent).toContain('Test Description');
  }));




it('should call enrollInCourse and navigate on enroll()', fakeAsync(() => {
  const notEnrolledCourse = { ...mockCourse, isEnrolled: false };
  studentServiceSpy.getCourseById.and.returnValue(of({ data: notEnrolledCourse }));
  paramMapSubject.next({ get: (key: string) => 'course1' });

  fixture = TestBed.createComponent(CourseDetailComponent);
  component = fixture.componentInstance;
  fixture.detectChanges();
  tick();

  component.enroll('course1');
  tick();

  expect(studentServiceSpy.enrollInCourse).toHaveBeenCalledWith('course1');
  expect(routerSpy.navigate).toHaveBeenCalledWith(['/student-dashboard/enrolled']);
  expect(toastrServiceSpy.success).toHaveBeenCalledWith('Enrolled successfully!');
}));


it('should handle enrollInCourse error', fakeAsync(() => {
  studentServiceSpy.enrollInCourse.and.returnValue(throwError(() => ({ error: { message: 'Already enrolled' } })));

  component.enroll('course1');
  tick();

  expect(toastrServiceSpy.error).toHaveBeenCalledWith('Enrollment failed: Already enrolled');
}));


  it('should call markFileAsCompleted and update local state', fakeAsync(() => {
   
    component.selectedVideo = { ...mockCourse.firstUploadedFile };
    component['courseSubject'].next({ ...mockCourse });
    fixture.detectChanges();
    component.markAsCompleted('file1');
    tick();
    fixture.detectChanges();

    expect(component.selectedVideo.isCompleted).toBeTrue();
   
    const updatedCourse = component['courseSubject'].value;
    expect(updatedCourse.uploadedFiles.find((f: any) => f.id === 'file1').isCompleted).toBeTrue();
  }));

  it('should update selectedVideo when playlist button is clicked', fakeAsync(() => {
    tick();
    fixture.detectChanges();

    // Simulate clicking the second file in the playlist
    const buttons = fixture.debugElement.queryAll(By.css('aside button'));
    expect(buttons.length).toBe(2);
    buttons[1].nativeElement.click();
    fixture.detectChanges();

    expect(component.selectedVideo.id).toBe('file2');
    expect(buttons[1].nativeElement.classList).toContain('bg-gray-100');
  }));

  it('should display preview section if not enrolled', fakeAsync(() => {
    const notEnrolledCourse = { ...mockCourse, isEnrolled: false };
    studentServiceSpy.getCourseById.and.returnValue(of({ data: notEnrolledCourse }));
    paramMapSubject.next({ get: (key: string) => 'course1' });
    fixture = TestBed.createComponent(CourseDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    tick();
    fixture.detectChanges();

    const preview = fixture.debugElement.query(By.css('h2'));
    expect(preview.nativeElement.textContent).toContain('Preview:');
    const enrollBtn = fixture.debugElement.query(By.css('button'));
    expect(enrollBtn.nativeElement.textContent).toContain('Enroll Now');
  }));

  it('should get a safe video url', () => {
    const url = component.getVideoUrl('video1.mp4');
    expect(sanitizer.bypassSecurityTrustResourceUrl).toBeDefined();
    expect(url).toBeTruthy();
  });
});
