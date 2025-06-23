
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class CreateCourseService {
  private courseIdSubject = new BehaviorSubject<string>('');
  private loadingSubject = new BehaviorSubject<boolean>(false);

  courseId$ = this.courseIdSubject.asObservable();
  loading$ = this.loadingSubject.asObservable();

  setCourseId(id: string) {
    this.courseIdSubject.next(id);
    console.log(this.courseId$);
  }

  getCourseId(): string {
    return this.courseIdSubject.getValue();
  }

  setLoading(isLoading: boolean) {
    this.loadingSubject.next(isLoading);
  }

  getLoading(): boolean {
    return this.loadingSubject.getValue();
  }

  reset() {
    this.courseIdSubject.next('');
    this.loadingSubject.next(false);
  }
}
