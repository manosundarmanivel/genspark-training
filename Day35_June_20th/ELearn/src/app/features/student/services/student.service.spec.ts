import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { StudentService } from './student.service';
import { HttpErrorResponse } from '@angular/common/http';

describe('StudentService', () => {
  let service: StudentService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [StudentService]
    });

    service = TestBed.inject(StudentService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('should fetch all courses', () => {
    const mockCourses = { data: [{ id: '1' }, { id: '2' }] };

    service.getAllCourses().subscribe(courses => {
      expect(courses.length).toBe(2);
    });

    const req = httpMock.expectOne('http://localhost:5295/api/v1/courses');
    expect(req.request.method).toBe('GET');
    req.flush(mockCourses);
  });

  it('should return empty array if search query is short', () => {
    service.searchCourses('a').subscribe(results => {
      expect(results).toEqual([]);
    });
  });

  it('should search courses with valid query', () => {
    const mockResponse = { data: [{ id: 'course1' }] };

    service.searchCourses('angular').subscribe(results => {
      expect(results.length).toBe(1);
    });

    const req = httpMock.expectOne('http://localhost:5295/api/v1/courses/search?query=angular');
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('should get course by ID', () => {
    const courseId = '123';
    const mockResponse = { data: { id: courseId } };

    service.getCourseById(courseId).subscribe(res => {
      expect(res.data.id).toBe(courseId);
    });

    const req = httpMock.expectOne(`http://localhost:5295/api/v1/courses/${courseId}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('should enroll in a course', () => {
    const courseId = '123';

    service.enrollInCourse(courseId).subscribe(response => {
      expect(response).toBeTruthy();
    });

    const req = httpMock.expectOne(`http://localhost:5295/api/v1/enrollments/${courseId}`);
    expect(req.request.method).toBe('POST');
    req.flush({ success: true });
  });

  it('should get enrolled courses', () => {
    const mockResponse = { data: [{ id: '1' }] };

    service.getEnrolledCourses().subscribe(courses => {
      expect(courses.length).toBe(1);
    });

    const req = httpMock.expectOne('http://localhost:5295/api/v1/enrollments');
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('should mark file as completed', () => {
    const fileId = 'file123';

    service.markFileAsCompleted(fileId).subscribe(res => {
      expect(res).toBeTruthy();
    });

    const req = httpMock.expectOne(`http://localhost:5295/api/v1/progress/complete/${fileId}`);
    expect(req.request.method).toBe('POST');
    req.flush({ success: true });
  });

  it('should fetch enrolled course details and course data for each', () => {
    const enrollmentsResponse = { data: [{ id: '1' }, { id: '2' }] };
    const course1 = { data: { id: '1', title: 'Course 1' } };
    const course2 = { data: { id: '2', title: 'Course 2' } };

    service.getEnrolledCourseDetails().subscribe(results => {
      expect(results.length).toBe(2);
      expect(results[0].id).toBe('1');
    });

    const enrollmentReq = httpMock.expectOne('http://localhost:5295/api/v1/enrollments');
    enrollmentReq.flush(enrollmentsResponse);

    const course1Req = httpMock.expectOne('http://localhost:5295/api/v1/courses/1');
    course1Req.flush(course1);

    const course2Req = httpMock.expectOne('http://localhost:5295/api/v1/courses/2');
    course2Req.flush(course2);
  });

  it('should skip a course if course detail fails in getEnrolledCourseDetails()', () => {
    const enrollmentsResponse = { data: [{ id: '1' }, { id: '2' }] };
    const course1 = { data: { id: '1', title: 'Course 1' } };

    service.getEnrolledCourseDetails().subscribe(results => {
      expect(results.length).toBe(1);
      expect(results[0].id).toBe('1');
    });

    const enrollmentReq = httpMock.expectOne('http://localhost:5295/api/v1/enrollments');
    enrollmentReq.flush(enrollmentsResponse);

    const course1Req = httpMock.expectOne('http://localhost:5295/api/v1/courses/1');
    course1Req.flush(course1);

    const course2Req = httpMock.expectOne('http://localhost:5295/api/v1/courses/2');
    course2Req.flush({}, { status: 404, statusText: 'Not Found' });
  });

  it('should catch error in getEnrolledCourseDetails()', () => {
    service.getEnrolledCourseDetails().subscribe({
      error: err => {
        expect(err.message).toBe('Could not load course details');
      }
    });

    const enrollmentReq = httpMock.expectOne('http://localhost:5295/api/v1/enrollments');
    enrollmentReq.flush({}, { status: 500, statusText: 'Server Error' });
  });

  it('should handle error in getCourseById', () => {
    service.getCourseById('bad-id').subscribe({
      error: (err: Error) => {
        expect(err.message).toContain('Server Error');
      }
    });

    const req = httpMock.expectOne('http://localhost:5295/api/v1/courses/bad-id');
    req.flush({ message: 'Course not found' }, { status: 404, statusText: 'Not Found' });
  });
});
