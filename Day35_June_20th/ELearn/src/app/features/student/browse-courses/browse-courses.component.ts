import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { StudentService } from '../services/student.service';
import { Observable, of, BehaviorSubject, combineLatest } from 'rxjs';
import { catchError, map, startWith, tap } from 'rxjs/operators';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';

interface Course {
  id: string;
  title: string;
  description: string;
  instructorId: string;
  uploadedFiles: any[];
  language: string;
  level: string;
  tags: string[];
  domain: string;
  price: number;
  thumbnailUrl: string;
}

@Component({
  selector: 'app-browse-courses',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './browse-courses.component.html',
})
export class BrowseCoursesComponent {
  private studentService = inject(StudentService);


  languageFilter$ = new BehaviorSubject<string>('');
  levelFilter$ = new BehaviorSubject<string>('');
  tagFilter$ = new BehaviorSubject<string>('');
  domainFilter$ = new BehaviorSubject<string>('');

  
  private allCourses$: Observable<Course[]> = this.studentService.getAllCourses().pipe(
    map(courses => courses),
    tap(courses => console.log('Loaded all courses:', courses)),
    catchError(err => {
      console.warn('Error loading courses:', err);
      return of([]);
    })
  );


  coursesState$: Observable<{
    courses: Course[];
    loading: boolean;
    error: string | null;
  }> = combineLatest([
    this.allCourses$,
    this.languageFilter$,
    this.levelFilter$,
    this.tagFilter$,
    this.domainFilter$
  ]).pipe(
    map(([courses, language, level, tag, domain]) => {
      const filtered = courses.filter(course => {
        const matchesLanguage = !language || course.language.toLowerCase() === language.toLowerCase();
        const matchesLevel = !level || course.level.toLowerCase() === level.toLowerCase();
        const matchesTag = !tag || course.tags.some(t => t.toLowerCase().includes(tag.toLowerCase()));
       const matchesDomain = !domain || course.domain.toLowerCase().includes(domain.toLowerCase());


        return matchesLanguage && matchesLevel && matchesTag && matchesDomain;
      });

      return { courses: filtered, loading: false, error: null };
    }),
    startWith({ courses: [], loading: true, error: null })
  );


  updateLanguage(value: string) {
    this.languageFilter$.next(value);
  }

  updateLevel(value: string) {
    this.levelFilter$.next(value);
  }

  updateTag(value: string) {
    this.tagFilter$.next(value);
  }

  updateDomain(value: string)
  {
    this.domainFilter$.next(value);
  }
}
