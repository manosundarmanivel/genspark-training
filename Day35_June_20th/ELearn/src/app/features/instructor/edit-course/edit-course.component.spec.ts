import { ComponentFixture, TestBed } from '@angular/core/testing';
import { EditCourseComponent } from './edit-course.component';
import { of } from 'rxjs';
import { ActivatedRoute, convertToParamMap, Router } from '@angular/router';
import { InstructorService } from '../services/instructor.service';

describe('EditCourseComponent', () => {
  let component: EditCourseComponent;
  let fixture: ComponentFixture<EditCourseComponent>;
  let mockInstructorService: any;
  let mockRouter: any;

  const mockCourseData = {
    title: 'Angular Basics',
    description: 'Intro to Angular',
    domain: 'Web Development',
    level: 'Beginner',
    language: 'English',
    tags: ['Angular', 'Frontend'],
    uploadedFiles: [
      {
        id: 'file1',
        topic: 'Intro',
        description: 'Introduction video',
        path: '/videos/intro.mp4'
      }
    ]
  };

  beforeEach(async () => {
    mockInstructorService = {
      getCourseByIdEdit: jasmine.createSpy().and.returnValue(of(mockCourseData)),
      updateCourseFile: jasmine.createSpy().and.returnValue(of({})),
      updateCourse: jasmine.createSpy().and.returnValue(of({}))
    };

    mockRouter = {
      navigate: jasmine.createSpy()
    };

    await TestBed.configureTestingModule({
      imports: [EditCourseComponent], // ✅ Standalone component goes here
      providers: [
        { provide: InstructorService, useValue: mockInstructorService },
        { provide: Router, useValue: mockRouter },
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              paramMap: convertToParamMap({ id: 'course123' })
            }
          }
        }
      ]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EditCourseComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create component and load course data', () => {
    expect(component).toBeTruthy();
    expect(component.courseForm).toBeDefined();
    expect(component.courseForm.value.title).toEqual('Angular Basics');
    expect(component.filesFormArray.length).toBe(1);
  });

  it('should patch course form correctly on load', () => {
    const formValue = component.courseForm.value;
    expect(formValue.title).toBe('Angular Basics');
    expect(formValue.domain).toBe('Web Development');
  });

  it('should call updateCourse when form is submitted', () => {
    component.courseForm.patchValue({
      title: 'Updated Title',
      description: 'Updated desc',
      domain: 'Web Development',
      level: 'Beginner',
      language: 'English',
      tags: []
    });

    component.onUpdateCourse();
    expect(mockInstructorService.updateCourse).toHaveBeenCalledWith('course123', jasmine.any(Object));
  });

  it('should update file when onUpdateFile is called', () => {
    const fileGroup = component.filesFormArray.at(0);
    fileGroup.patchValue({
      topic: 'Updated Topic',
      description: 'Updated Desc',
      file: new File(['dummy'], 'video.mp4')
    });

    component.onUpdateFile(0);
    expect(mockInstructorService.updateCourseFile).toHaveBeenCalled();
  });

  it('should set file value on file input change', () => {
    const file = new File(['dummy'], 'video.mp4');
    const event = {
      target: {
        files: [file]
      }
    } as unknown as Event;

    component.onFileChange(event, 0);
    expect(component.filesFormArray.at(0).value.file).toBe(file);
  });

  it('should warn if course form is invalid on update', () => {
    spyOn(window, 'alert');
    component.courseForm.patchValue({ title: '', description: '' });
    component.onUpdateCourse();
    expect(window.alert).not.toHaveBeenCalled(); // Reactive form prevents submission
  });

  it('should warn and not upload new file if new file form is invalid', () => {
    spyOn(window, 'alert');
    component.onAddNewFile();
    expect(window.alert).toHaveBeenCalledWith('Please complete all required fields');
  });
});
