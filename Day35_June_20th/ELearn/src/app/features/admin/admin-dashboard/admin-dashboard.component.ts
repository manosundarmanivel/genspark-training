import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BaseChartDirective } from 'ng2-charts';
import { ChartData, ChartOptions } from 'chart.js';
import { Observable } from 'rxjs';
import { map, shareReplay } from 'rxjs/operators';

import { AdminService } from '../services/admin.service';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, BaseChartDirective],
  templateUrl: './admin-dashboard.component.html'
})
export class AdminDashboardComponent {
  domainOptions = ['Artificial Intelligence', 'Web Development', 'Data Science', 'Cybersecurity', 'Cloud Computing'];
  levelOptions = ['Beginner', 'Intermediate', 'Advanced'];
  languageOptions = ['English', 'Hindi', 'Spanish', 'French', 'German'];

  chartOptions: ChartOptions<'pie'> = {
    responsive: true,
    plugins: {
      legend: {
        position: 'bottom'
      }
    }
  };

  data$!: Observable<{ users: any[]; courses: any[] }>;
  totalCourses$!: Observable<number>;
  studentCount$!: Observable<number>;
  instructorCount$!: Observable<number>;
  domainChartData$!: Observable<ChartData<'pie'>>;
  levelChartData$!: Observable<ChartData<'pie'>>;
  languageChartData$!: Observable<ChartData<'pie'>>;

  constructor(private adminService: AdminService) {
    this.data$ = this.adminService.getAllUsersAndCourses().pipe(
      map(({ users, courses }) => ({
        users: users?.data || [],
        courses: courses?.data || []
      })),
      shareReplay(1) // avoids multiple subscriptions
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
