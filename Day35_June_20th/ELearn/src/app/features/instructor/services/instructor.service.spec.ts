import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { InstructorService } from './instructor.service';
import { HttpErrorResponse } from '@angular/common/http';

describe('InstructorService', () => {
  let service: InstructorService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [InstructorService]
    });

    service = TestBed.inject(InstructorService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('should add a course', () => {
    const formData = new FormData();
    const mockResponse = { success: true };

    service.addCourse(formData).subscribe(res => {
      expect(res).toEqual(mockResponse);
    });

    const req = httpMock.expectOne('http://localhost:5295/api/v1/courses');
    expect(req.request.method).toBe('POST');
    req.flush(mockResponse);
  });

  it('should upload a file', () => {
    const formData = new FormData();
    const mockResponse = { uploaded: true };

    service.uploadFile(formData).subscribe(res => {
      expect(res).toEqual(mockResponse);
    });

    const req = httpMock.expectOne('http://localhost:5295/api/v1/files/upload');
    expect(req.request.method).toBe('POST');
    req.flush(mockResponse);
  });

  it('should get instructor courses', () => {
    const mockResponse = { data: [{ id: '1' }, { id: '2' }] };

    service.getInstructorCourses(1, 10).subscribe(data => {
      expect(data.length).toBe(2);
    });

    const req = httpMock.expectOne(r =>
      r.url === 'http://localhost:5295/api/v1/courses/instructor' &&
      r.params.get('page') === '1' &&
      r.params.get('pageSize') === '10'
    );
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('should get course by id', () => {
    const courseId = 'course123';
    const mockResponse = { data: { id: courseId } };

    service.getCourseById(courseId).subscribe(data => {
      expect(data.id).toBe(courseId);
    });

    const req = httpMock.expectOne(`http://localhost:5295/api/v1/courses/${courseId}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('should get course by id for edit', () => {
    const courseId = 'course123';
    const mockResponse = { data: { id: courseId } };

    service.getCourseByIdEdit(courseId).subscribe(data => {
      expect(data.id).toBe(courseId);
    });

    const req = httpMock.expectOne(`http://localhost:5295/api/v1/courses/instructor/${courseId}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('should get course files', () => {
    const courseId = 'course123';
    const mockResponse = { data: [{ fileId: 'f1' }] };

    service.getCourseFiles(courseId).subscribe(data => {
      expect(data.length).toBe(1);
    });

    const req = httpMock.expectOne(`http://localhost:5295/api/v1/student/files/${courseId}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('should update course file', () => {
    const fileId = 'file123';
    const formData = new FormData();
    const mockResponse = { success: true };

    service.updateCourseFile(fileId, formData).subscribe(res => {
      expect(res).toEqual(mockResponse);
    });

    const req = httpMock.expectOne(`http://localhost:5295/api/v1/files/${fileId}`);
    expect(req.request.method).toBe('PUT');
    req.flush(mockResponse);
  });

  it('should update a file', () => {
    const fileId = 'file123';
    const formData = new FormData();
    const mockResponse = { updated: true };

    service.updateFile(fileId, formData).subscribe(res => {
      expect(res).toEqual(mockResponse);
    });

    const req = httpMock.expectOne(`http://localhost:5295/api/v1/files/${fileId}`);
    expect(req.request.method).toBe('PUT');
    req.flush(mockResponse);
  });

  it('should update course', () => {
    const courseId = 'course123';
    const courseData = { title: 'Updated Title' };

    service.updateCourse(courseId, courseData).subscribe(res => {
      expect(res).toEqual({ success: true });
    });

    const req = httpMock.expectOne(`http://localhost:5295/api/v1/courses/${courseId}`);
    expect(req.request.method).toBe('PUT');
    req.flush({ success: true });
  });

  it('should handle GET with dynamic URL', () => {
    const url = 'http://localhost:5295/api/v1/custom';
    const mockResponse = { custom: true };

    service.get(url).subscribe(res => {
      expect(res).toEqual(mockResponse);
    });

    const req = httpMock.expectOne(url);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('should handle error gracefully', () => {
    const formData = new FormData();

    service.addCourse(formData).subscribe({
      error: (err: Error) => {
        expect(err.message).toContain('Server Error');
      }
    });

    const req = httpMock.expectOne('http://localhost:5295/api/v1/courses');
    req.flush({ message: 'Bad Request' }, { status: 400, statusText: 'Bad Request' });
  });
});
