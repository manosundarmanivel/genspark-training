import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { HttpClient, HttpParams } from '@angular/common/http';
import { catchError, map, Observable, of, tap } from 'rxjs';
import { InstructorService } from '../services/instructor.service';

@Component({
  selector: 'app-instructor-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './instructor-dashboard.component.html',
})
export class InstructorDashboardComponent implements OnInit {
  dashboardData$!: Observable<{
    totalCourses: number;
    totalUploads: number;
    totalEnrollments: number;
    recentCourses: any[];
  }>;


  constructor(private instructorService: InstructorService) {}

  ngOnInit(): void {
    this.dashboardData$ = this.instructorService.getInstructorCourses(1, 100).pipe(
      tap(res => console.log('Instructor Courses API Response:', res)),
      map((courses: any[]) => {
        const totalCourses = courses.length;
        const totalUploads = courses.reduce((sum, c) => sum + (c.uploadedFiles?.length || 0), 0);
        const totalEnrollments = courses.reduce((sum, c) => sum + (c.enrolledStudents
?.length || 0), 0);
        const recentCourses = courses.slice(0, 2); // top 2 only

        return { totalCourses, totalUploads, totalEnrollments, recentCourses };
      }),
      catchError(err => {
        console.error('Dashboard data load failed', err);
        return of({ totalCourses: 0, totalUploads: 0, totalEnrollments: 0, recentCourses: [] });
      })
    );
  }
}
