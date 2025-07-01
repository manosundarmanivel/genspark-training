import { TestBed } from '@angular/core/testing';
import { AdminService } from './admin.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

describe('AdminService', () => {
  let service: AdminService;
  let httpMock: HttpTestingController;

  const mockUsers = {
    data: [
      { id: 'u1', username: 'john' },
      { id: 'u2', username: 'admin' }
    ]
  };

  const mockCourses = {
    data: [
      { id: 'c1', title: 'Course 1' },
      { id: 'c2', title: 'Course 2' }
    ]
  };

  const mockDetailedCourse = (id: string) => ({
    id,
    title: `Course ${id}`,
    description: `Detailed info for ${id}`
  });

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [AdminService]
    });

    service = TestBed.inject(AdminService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch all users and detailed courses', () => {
    service.getUsersAndDetailedCourses().subscribe(result => {
      expect(result.users.length).toBe(2);
      expect(result.courses.length).toBe(2);
      expect(result.courses[0].description).toContain('Detailed info');
    });

    // Main users and courses fetch
    const usersReq = httpMock.expectOne('http://localhost:5295/api/v1/admin/users');
    usersReq.flush(mockUsers);

    const coursesReq = httpMock.expectOne('http://localhost:5295/api/v1/admin/courses');
    coursesReq.flush(mockCourses);

    // Detailed courses
    mockCourses.data.forEach(c => {
      const detailReq = httpMock.expectOne(`http://localhost:5295/api/v1/courses/${c.id}`);
      detailReq.flush(mockDetailedCourse(c.id));
    });
  });

  it('should fallback to original course if detailed course fetch fails', () => {
    service.getUsersAndDetailedCourses().subscribe(result => {
      expect(result.courses.length).toBe(2);
      expect(result.courses[0].title).toBe('Course 1');
    });

    const usersReq = httpMock.expectOne('http://localhost:5295/api/v1/admin/users');
    usersReq.flush(mockUsers);

    const coursesReq = httpMock.expectOne('http://localhost:5295/api/v1/admin/courses');
    coursesReq.flush(mockCourses);

    const failReq = httpMock.expectOne(`http://localhost:5295/api/v1/courses/c1`);
    failReq.error(new ErrorEvent('Network error'));

    const detailReq = httpMock.expectOne(`http://localhost:5295/api/v1/courses/c2`);
    detailReq.flush(mockDetailedCourse('c2'));
  });

  it('should fetch all users and raw courses', () => {
    service.getAllUsersAndCourses().subscribe(res => {
      expect(res.users.data.length).toBe(2);
      expect(res.courses.data.length).toBe(2);
    });

    httpMock.expectOne('http://localhost:5295/api/v1/admin/users').flush(mockUsers);
    httpMock.expectOne('http://localhost:5295/api/v1/admin/courses').flush(mockCourses);
  });

  it('should fetch all users', () => {
    service.getAllUsers().subscribe(res => {
      expect(res.data.length).toBe(2);
    });

    httpMock.expectOne('http://localhost:5295/api/v1/admin/users').flush(mockUsers);
  });

  it('should fetch user by ID', () => {
    const userId = 'u1';

    service.getUserById(userId).subscribe(res => {
      expect(res.id).toBe(userId);
    });

    httpMock.expectOne(`http://localhost:5295/api/v1/admin/users/${userId}`).flush({ id: userId });
  });

  it('should enable a user', () => {
    const userId = 'u1';

    service.enableUser(userId).subscribe(res => {
      expect(res.success).toBeTrue();
    });

    httpMock.expectOne(`http://localhost:5295/api/v1/admin/users/${userId}/enable`).flush({ success: true });
  });

  it('should disable a user', () => {
    const userId = 'u2';

    service.disableUser(userId).subscribe(res => {
      expect(res.success).toBeTrue();
    });

    httpMock.expectOne(`http://localhost:5295/api/v1/admin/users/${userId}/disable`).flush({ success: true });
  });

  it('should fetch all courses', () => {
    service.getAllCourses().subscribe(res => {
      expect(res.data.length).toBe(2);
    });

    httpMock.expectOne('http://localhost:5295/api/v1/admin/courses').flush(mockCourses);
  });

  it('should enable a course', () => {
    const courseId = 'c1';

    service.enableCourse(courseId).subscribe(res => {
      expect(res.success).toBeTrue();
    });

    httpMock.expectOne(`http://localhost:5295/api/v1/admin/courses/${courseId}/enable`).flush({ success: true });
  });

  it('should disable a course', () => {
    const courseId = 'c2';

    service.disableCourse(courseId).subscribe(res => {
      expect(res.success).toBeTrue();
    });

    httpMock.expectOne(`http://localhost:5295/api/v1/admin/courses/${courseId}/disable`).flush({ success: true });
  });

  it('should fetch course by ID', () => {
    const courseId = 'c1';

    service.getCourseById(courseId).subscribe(res => {
      expect(res.id).toBe(courseId);
    });

    httpMock.expectOne(`http://localhost:5295/api/v1/courses/${courseId}`).flush({ id: courseId });
  });
});
