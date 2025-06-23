import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormControl } from '@angular/forms';
import { CommonModule, AsyncPipe } from '@angular/common';
import { InstructorService } from '../services/instructor.service';
import { CreateCourseService } from './create-course.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-create-course',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, AsyncPipe],
  templateUrl: './create-course.component.html',
})
export class CreateCourseComponent {
  step = 1;
  courseForm: FormGroup;
  fileForm: FormGroup;
  loading$: any;

  constructor(
    private fb: FormBuilder,
    private instructorService: InstructorService,
    private createCourseService: CreateCourseService,
    private router: Router
  ) {
    this.loading$ = this.createCourseService.loading$;
    this.courseForm = this.fb.group({
      title: ['', Validators.required],
      description: ['', Validators.required],
      thumbnailUrl: ['', Validators.required],
      domain: ['', Validators.required],
      level: ['', Validators.required],
      language: ['', Validators.required],
      tags: ['', Validators.required] // Will convert to array on submit
    });

    this.fileForm = this.fb.group({
      topic: ['', Validators.required],
      description: ['', Validators.required],
      file: [null, Validators.required],
    });
  }

  submitCourse(): void {
    if (this.courseForm.invalid || this.createCourseService.getLoading()) return;

    this.createCourseService.setLoading(true);

    // Convert comma-separated tags string to array
    const formValue = { ...this.courseForm.value };
    formValue.tags = formValue.tags.split(',').map((tag: string) => tag.trim());

    this.instructorService.addCourse(formValue).subscribe({
      next: (res) => {
        const id = res?.data?.id;
        if (id) {
          this.createCourseService.setCourseId(id);
          this.step = 2;
        } else {
          alert('No Course ID returned. Try again.');
        }
        this.createCourseService.setLoading(false);
      },
      error: (err) => {
        alert('Course creation failed: ' + err.message);
        this.createCourseService.setLoading(false);
      }
    });
  }

  onFileChange(event: Event): void {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (file) {
      this.fileForm.patchValue({ file });
      this.fileForm.get('file')?.updateValueAndValidity();
    }
  }

  uploadContent(): void {
    if (this.fileForm.invalid || this.createCourseService.getLoading()) return;

    const courseId = this.createCourseService.getCourseId();
    if (!courseId) return alert('Missing course ID.');

    const file: File = this.fileForm.get('file')?.value;
    if (!file) return alert('File is missing');

    const formData = new FormData();
    formData.append('File', file, file.name);
    formData.append('CourseId', courseId);
    formData.append('Topic', this.fileForm.value.topic);
    formData.append('Description', this.fileForm.value.description);

    this.createCourseService.setLoading(true);

    this.instructorService.uploadFile(formData).subscribe({
      next: (res) => {
        if (res?.success) {
          alert('Upload successful!');
          this.resetForms();
          this.router.navigate(['/instructor-dashboard']);
        } else {
          alert('Upload failed on server.');
        }
        this.createCourseService.setLoading(false);
      },
      error: (err) => {
        alert('Upload failed: ' + err.message);
        this.createCourseService.setLoading(false);
      }
    });
  }

  private resetForms(): void {
    this.courseForm.reset();
    this.fileForm.reset();
    this.createCourseService.reset();
    this.step = 1;
  }
}
