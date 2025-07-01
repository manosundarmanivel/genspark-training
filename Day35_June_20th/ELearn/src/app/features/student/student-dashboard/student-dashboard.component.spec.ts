import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { StudentDashboardComponent } from './student-dashboard.component';
import { RouterTestingModule } from '@angular/router/testing';
import { of } from 'rxjs';
import { StudentService } from '../services/student.service';
import { AuthService } from '../../auth/auth.service';
import { By } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';

// Mock Data
const mockCourses = [
  {
    id: '1',
    title: 'Angular Basics',
    instructorName: 'John Doe',
    isCompleted: false
  },
  {
    id: '2',
    title: 'Advanced RxJS',
    instructorName: 'Jane Smith',
    isCompleted: true
  }
];

// Mock Services
class MockStudentService {
  getEnrolledCourseDetails() {
    return of(mockCourses);
  }
}

class MockAuthService {
  logout = jasmine.createSpy('logout');
}

describe('StudentDashboardComponent (standalone)', () => {
  let component: StudentDashboardComponent;
  let fixture: ComponentFixture<StudentDashboardComponent>;
  let authService: AuthService;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      imports: [
        StudentDashboardComponent, // ✅ Standalone component imported directly
        CommonModule,
        RouterTestingModule
      ],
      providers: [
        { provide: StudentService, useClass: MockStudentService },
        { provide: AuthService, useClass: MockAuthService }
      ]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StudentDashboardComponent);
    component = fixture.componentInstance;
    authService = TestBed.inject(AuthService);
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should load enrolled courses and calculate stats', (done) => {
    component.totalCourses$.subscribe((total) => {
      expect(total).toBe(2);
    });

    component.completedCourses$.subscribe((completed) => {
      expect(completed).toBe(1);
      done();
    });
  });

  it('should display total and completed course counts', (done) => {
    fixture.whenStable().then(() => {
      fixture.detectChanges();
      const statValues = fixture.debugElement.queryAll(By.css('.text-5xl'));
      expect(statValues[0].nativeElement.textContent.trim()).toBe('2');
      expect(statValues[1].nativeElement.textContent.trim()).toBe('1');
      done();
    });
  });

  it('should render continue learning cards', (done) => {
    fixture.whenStable().then(() => {
      fixture.detectChanges();
      const courseCards = fixture.debugElement.queryAll(By.css('h4'));
      expect(courseCards.length).toBeGreaterThan(0);
      expect(courseCards[0].nativeElement.textContent).toContain('Angular Basics');
      done();
    });
  });

  it('should render "My Courses" list', (done) => {
    fixture.whenStable().then(() => {
      fixture.detectChanges();
      const listItems = fixture.debugElement.queryAll(By.css('ul li h4'));
      expect(listItems.length).toBe(2);
      expect(listItems[1].nativeElement.textContent).toContain('Advanced RxJS');
      done();
    });
  });

  it('should call AuthService.logout() on logout()', () => {
    component.logout();
    expect(authService.logout).toHaveBeenCalled();
  });
});
