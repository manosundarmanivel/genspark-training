import { ComponentFixture, TestBed, fakeAsync, tick, waitForAsync } from '@angular/core/testing';
import { InstructorDashboardComponent } from './instructor-dashboard.component';
import { InstructorService } from '../services/instructor.service';
import { of, throwError } from 'rxjs';
import { By } from '@angular/platform-browser';
import { RouterTestingModule } from '@angular/router/testing';

describe('InstructorDashboardComponent', () => {
  let component: InstructorDashboardComponent;
  let fixture: ComponentFixture<InstructorDashboardComponent>;
  let mockInstructorService: any;

  const mockCourses = [
    {
      id: 'c1',
      title: 'Angular Masterclass',
      uploadedFiles: [{}, {}],
      enrolledStudents: [{}, {}, {}]
    },
    {
      id: 'c2',
      title: 'RxJS Deep Dive',
      uploadedFiles: [{}],
      enrolledStudents: [{}]
    }
  ];

  beforeEach(waitForAsync(() => {
    mockInstructorService = {
      getInstructorCourses: jasmine.createSpy().and.returnValue(of(mockCourses))
    };

    TestBed.configureTestingModule({
      imports: [
        InstructorDashboardComponent, // ✅ use standalone import
        RouterTestingModule
      ],
      providers: [
        { provide: InstructorService, useValue: mockInstructorService }
      ]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InstructorDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the dashboard component', () => {
    expect(component).toBeTruthy();
  });

  it('should load dashboard data correctly', fakeAsync(() => {
    fixture.detectChanges();
    tick(); // resolve observables
    fixture.detectChanges();

    const compiled = fixture.debugElement.nativeElement;

    const totalCourses = compiled.querySelector('p.text-blue-900');
    const totalEnrollments = compiled.querySelector('p.text-indigo-900');
    const totalUploads = compiled.querySelector('p.text-teal-900');

    expect(totalCourses.textContent).toContain('2');
    expect(totalEnrollments.textContent).toContain('4');
    expect(totalUploads.textContent).toContain('3');
  }));

  it('should display recent course titles', fakeAsync(() => {
    fixture.detectChanges();
    tick();
    fixture.detectChanges();

    const courseItems = fixture.debugElement.queryAll(By.css('li'));
    expect(courseItems.length).toBe(2);

    const courseTitles = courseItems.map(item =>
      item.query(By.css('h4')).nativeElement.textContent.trim()
    );
    expect(courseTitles).toContain('Angular Masterclass');
    expect(courseTitles).toContain('RxJS Deep Dive');
  }));

  it('should contain action buttons with correct text', fakeAsync(() => {
    fixture.detectChanges();
    tick();
    fixture.detectChanges();

    const buttons = fixture.debugElement.queryAll(By.css('button'));
    const createBtn = buttons[0].nativeElement;
    const uploadBtn = buttons[1].nativeElement;

    expect(createBtn.textContent).toContain('Create New Course');
    expect(uploadBtn.textContent).toContain('Upload Course Material');
  }));

  it('should show fallback values on error', fakeAsync(() => {
    mockInstructorService.getInstructorCourses.and.returnValue(throwError(() => new Error('API failed')));
    fixture = TestBed.createComponent(InstructorDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    tick();
    fixture.detectChanges();

    const compiled = fixture.debugElement.nativeElement;
    expect(compiled.querySelector('p.text-blue-900').textContent).toContain('0');
    expect(compiled.querySelector('p.text-indigo-900').textContent).toContain('0');
    expect(compiled.querySelector('p.text-teal-900').textContent).toContain('0');
  }));
});
