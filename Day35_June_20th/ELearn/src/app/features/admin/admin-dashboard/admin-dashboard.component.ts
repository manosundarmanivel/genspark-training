import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BaseChartDirective } from 'ng2-charts';
import { ChartData, ChartOptions } from 'chart.js';
import { Observable, of } from 'rxjs';
import { map, shareReplay } from 'rxjs/operators';
import { AdminService, EnrollmentStats } from '../services/admin.service';

interface Stat {
  date: string;
  enrollmentCount: number;
}

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, BaseChartDirective],
  templateUrl: './admin-dashboard.component.html'
})
export class AdminDashboardComponent {
  // Chart configuration
  chartOptions: ChartOptions<'pie'> = {
    responsive: true,
    plugins: {
      legend: {
        position: 'bottom'
      }
    }
  };

  lineChartOptions: ChartOptions<'line'> = {
    responsive: true,
    plugins: {
      legend: {
        position: 'bottom'
      }
    }
  };

  // Select options
  domainOptions = ['Artificial Intelligence', 'Web Development', 'Data Science', 'Cybersecurity', 'Cloud Computing'];
  levelOptions = ['Beginner', 'Intermediate', 'Advanced'];
  languageOptions = ['English', 'Hindi', 'Spanish', 'French', 'German'];

  // State
  selectedDays = 7;

  // Observables
  data$!: Observable<{ users: any[]; courses: any[] }>;
  totalCourses$!: Observable<number>;
  studentCount$!: Observable<number>;
  instructorCount$!: Observable<number>;
  domainChartData$!: Observable<ChartData<'pie'>>;
  levelChartData$!: Observable<ChartData<'pie'>>;
  languageChartData$!: Observable<ChartData<'pie'>>;
  enrollmentStatsChartData$!: Observable<ChartData<'line'>>;

  constructor(private adminService: AdminService) {
    this.loadUserCourseData();
    this.loadEnrollmentStats();
  }

  loadUserCourseData() {
    this.data$ = this.adminService.getAllUsersAndCourses().pipe(
      map(({ users, courses }) => ({
        users: users?.data || [],
        courses: courses?.data || []
      })),
      shareReplay(1)
    );

    this.totalCourses$ = this.data$.pipe(
      map(({ courses }) => courses.length)
    );

    this.studentCount$ = this.data$.pipe(
      map(({ users }) => users.filter(user => user.role?.name === 'Student').length)
    );

    this.instructorCount$ = this.data$.pipe(
      map(({ users }) => users.filter(user => user.role?.name === 'Instructor').length)
    );

    this.domainChartData$ = this.data$.pipe(
      map(({ courses }) => this.buildChartData(this.domainOptions, courses, 'domain'))
    );

    this.levelChartData$ = this.data$.pipe(
      map(({ courses }) => this.buildChartData(this.levelOptions, courses, 'level'))
    );

    this.languageChartData$ = this.data$.pipe(
      map(({ courses }) => this.buildChartData(this.languageOptions, courses, 'language'))
    );
  }

  loadEnrollmentStats() {
    this.enrollmentStatsChartData$ = this.adminService.getDailyEnrollmentStats(this.selectedDays).pipe(
      map((res: any) => Array.isArray(res?.data) ? res.data : []),
      map((stats: Stat[]) => ({
        labels: stats.map((s: Stat) =>
          new Date(s.date).toLocaleDateString('en-IN', { day: 'numeric', month: 'short' })
        ),
        datasets: [
          {
            data: stats.map((s: Stat) => s.enrollmentCount),
            label: 'Enrollments',
            fill: true,
            tension: 0.4,
            borderColor: '#3b82f6',
            backgroundColor: 'rgba(59, 130, 246, 0.2)',
            pointBackgroundColor: '#3b82f6'
          }
        ]
      }))
    );
  }

  private buildChartData(options: string[], courses: any[], key: string): ChartData<'pie'> {
    const data = options.map(opt => courses.filter(c => c[key] === opt).length);
    return {
      labels: options,
      datasets: [
        {
          data,
          backgroundColor: ['#f87171', '#60a5fa', '#34d399', '#fbbf24', '#a78bfa']
        }
      ]
    };
  }
}
