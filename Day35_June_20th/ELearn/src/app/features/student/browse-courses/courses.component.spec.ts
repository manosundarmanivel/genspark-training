import { ComponentFixture, TestBed } from '@angular/core/testing';
import { BrowseCoursesComponent } from './browse-courses.component';
import { StudentService } from '../services/student.service';
import { of, throwError, firstValueFrom } from 'rxjs';
import { By } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';

describe('BrowseCoursesComponent', () => {
  let component: BrowseCoursesComponent;
  let fixture: ComponentFixture<BrowseCoursesComponent>;
  let mockStudentService: jasmine.SpyObj<StudentService>;

  const mockCourses = [
    {
      id: '1',
      title: 'Angular Basics',
      description: 'Learn the basics of Angular',
      instructorId: 'abc',
      uploadedFiles: [],
      language: 'English',
      level: 'Beginner',
      tags: ['angular', 'frontend'],
      domain: 'Web Development',
      thumbnailUrl: ''
    },
    {
      id: '2',
      title: 'Advanced Angular',
      description: 'Deep dive into Angular topics',
      instructorId: 'abc',
      uploadedFiles: [],
      language: 'English',
      level: 'Advanced',
      tags: ['angular'],
      domain: 'Web Development',
      thumbnailUrl: ''
    }
  ];

  beforeEach(async () => {
    mockStudentService = jasmine.createSpyObj('StudentService', ['getAllCourses']);
    mockStudentService.getAllCourses.and.returnValue(of(mockCourses));

    await TestBed.configureTestingModule({
      imports: [
        FormsModule,
        RouterTestingModule,
        BrowseCoursesComponent // Standalone component
      ],
      providers: [
        { provide: StudentService, useValue: mockStudentService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(BrowseCoursesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should load and display courses', async () => {
    await fixture.whenStable();
    fixture.detectChanges();
    const courseCards = fixture.debugElement.queryAll(By.css('.bg-white'));
    expect(courseCards.length).toBe(2);
    expect(courseCards[0].nativeElement.textContent).toContain('Angular Basics');
  });

//   it('should filter courses by language', async () => {
//     component.updateLanguage('English');
//     fixture.detectChanges();
//     await fixture.whenStable();

//     const filteredState = await firstValueFrom(component.coursesState$);
//     expect(filteredState.courses.length).toBe(2);
//   });

//   it('should filter courses by level', async () => {
//     component.updateLevel('Beginner');
//     fixture.detectChanges();
//     await fixture.whenStable();

//     const filteredState = await firstValueFrom(component.coursesState$);
//     expect(filteredState.courses.length).toBe(1);
//     expect(filteredState.courses[0]?.title).toBe('Angular Basics');
//   });

//   it('should show error message when service fails', async () => {
//     mockStudentService.getAllCourses.and.returnValue(throwError(() => new Error('Failed to fetch')));

//     const errorFixture = TestBed.createComponent(BrowseCoursesComponent);
//     const errorComponent = errorFixture.componentInstance;
//     errorFixture.detectChanges();
//     await errorFixture.whenStable();

//     const errorState = await firstValueFrom(errorComponent.coursesState$);
//     expect(errorState.courses.length).toBe(0);
//     expect(errorState.error).toContain('Failed to fetch');
//   });

  it('should show "no courses found" when filter returns empty', async () => {
    component.updateLanguage('Hindi'); // No Hindi courses
    fixture.detectChanges();
    await fixture.whenStable();

    const filteredState = await firstValueFrom(component.coursesState$);
    expect(filteredState.courses.length).toBe(0);
  });
});
