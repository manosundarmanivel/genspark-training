import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { DashboardLayoutComponent } from './layout/dashboard-layout/dashboard-layout.component';
import { InstructorDashboardComponent } from './features/instructor/instructor-dashboard/instructor-dashboard.component';
import { RoleGuard } from './core/guards/auth.guard';
import { CreateCourseComponent } from './features/instructor/create-course/create-course.component';
import { StudentDashboardComponent } from './features/student/student-dashboard/student-dashboard.component';
import { MyCoursesComponent } from './features/instructor/my-courses/my-courses.component';
import { BrowseCoursesComponent } from './features/student/browse-courses/browse-courses.component';
import { CourseDetailComponent } from './features/student/course-detail/course-detail.component';
import { EnrolledCoursesComponent } from './features/student/enrolled-courses/enrolled-courses.component';
import { AdminDashboardComponent } from './features/admin/admin-dashboard/admin-dashboard.component';
import { ProfileComponent } from './shared/components/profile/profile.component';

export const routes: Routes = [
{ path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },

//   { path: 'admin-dashboard', component: AdminDashboardComponent },
//   { path: 'instructor-dashboard', component: InstructorDashboardComponent },
//   { path: 'student-dashboard', component: StudentDashboardComponent },
{
  path: 'instructor-dashboard',
  component: DashboardLayoutComponent,
  canActivate: [RoleGuard],
  data: { expectedRole: 'Instructor' },
  children: [
    { path: '', component: InstructorDashboardComponent },
    { path: 'create-course', component: CreateCourseComponent },
    { path: 'my-courses', component: MyCoursesComponent },
    {path:'profile', component:ProfileComponent}
  ]
},
{
  path: 'student-dashboard',
  component: DashboardLayoutComponent,
  canActivate: [RoleGuard],
  data: { expectedRole: 'Student' },
  children: [
    { path: '', component: StudentDashboardComponent },
    {path: 'browse', component: BrowseCoursesComponent},
    {path: 'course-detail/:courseId', component:CourseDetailComponent},
    {path: 'enrolled', component:EnrolledCoursesComponent}
   
    // { path: 'my-courses', component: MyCoursesComponent }
  ]
},

{ 
  path: 'admin-dashboard',
  component: AdminDashboardComponent,
  canActivate: [RoleGuard],
  data: { expectedRole: 'Admin' }
},

  { path: '', redirectTo: 'login', pathMatch: 'full' }
];







