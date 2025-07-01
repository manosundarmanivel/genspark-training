// import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
// import { AdminDashboardComponent } from './admin-dashboard.component';
// import { AdminService } from '../services/admin.service';
// import { of } from 'rxjs';
// import { RouterTestingModule } from '@angular/router/testing';
// import { BaseChartDirective } from 'ng2-charts';

// // Mock Data
// const mockUsers = [
//   { role: { name: 'Student' } },
//   { role: { name: 'Instructor' } },
//   { role: { name: 'Student' } }
// ];

// const mockCourses = [
//   { domain: 'Web Development', level: 'Beginner', language: 'English' },
//   { domain: 'Web Development', level: 'Advanced', language: 'Hindi' },
//   { domain: 'Artificial Intelligence', level: 'Intermediate', language: 'English' }
// ];

// // Mock AdminService
// class MockAdminService {
//   getAllUsersAndCourses() {
//     return of({
//       users: { data: mockUsers },
//       courses: { data: mockCourses }
//     });
//   }
// }

// describe('AdminDashboardComponent (standalone)', () => {
//   let component: AdminDashboardComponent;
//   let fixture: ComponentFixture<AdminDashboardComponent>;

//   beforeEach(waitForAsync(() => {
//     TestBed.configureTestingModule({
//       imports: [
//         AdminDashboardComponent,
//         RouterTestingModule
//       ],
//       providers: [
//         { provide: AdminService, useClass: MockAdminService }
//       ]
//     }).compileComponents();
//   }));

//   beforeEach(() => {
//     fixture = TestBed.createComponent(AdminDashboardComponent);
//     component = fixture.componentInstance;
//     fixture.detectChanges();
//   });

//   it('should create the component', () => {
//     expect(component).toBeTruthy();
//   });

//   it('should calculate total courses', (done) => {
//     component.totalCourses$.subscribe(count => {
//       expect(count).toBe(3);
//       done();
//     });
//   });

//   it('should calculate student and instructor counts', (done) => {
//     component.studentCount$.subscribe(count => {
//       expect(count).toBe(2);
//     });

//     component.instructorCount$.subscribe(count => {
//       expect(count).toBe(1);
//       done();
//     });
//   });

//   it('should build domain chart data correctly', (done) => {
//     component.domainChartData$.subscribe(chartData => {
//       expect(chartData.labels).toContain('Web Development');
//       const webDevIndex = (chartData.labels as string[]).indexOf('Web Development');
//       expect(chartData.datasets[0].data[webDevIndex]).toBe(2);
//       done();
//     });
//   });

//   it('should build level chart data correctly', (done) => {
//     component.levelChartData$.subscribe(chartData => {
//       const levels = chartData.labels as string[];
//       const beginnerIndex = levels.indexOf('Beginner');
//       const advancedIndex = levels.indexOf('Advanced');
//       const intermediateIndex = levels.indexOf('Intermediate');

//       expect(chartData.datasets[0].data[beginnerIndex]).toBe(1);
//       expect(chartData.datasets[0].data[advancedIndex]).toBe(1);
//       expect(chartData.datasets[0].data[intermediateIndex]).toBe(1);
//       done();
//     });
//   });

//   it('should build language chart data correctly', (done) => {
//     component.languageChartData$.subscribe(chartData => {
//       const langs = chartData.labels as string[];
//       const englishIndex = langs.indexOf('English');
//       const hindiIndex = langs.indexOf('Hindi');

//       expect(chartData.datasets[0].data[englishIndex]).toBe(2);
//       expect(chartData.datasets[0].data[hindiIndex]).toBe(1);
//       done();
//     });
//   });
// });
