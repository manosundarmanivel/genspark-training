import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  Validators,
  ReactiveFormsModule,
  FormsModule,
} from '@angular/forms';
import { CommonModule } from '@angular/common';
import { InstructorService } from '../services/instructor.service';
import { CreateCourseService } from './create-course.service';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-create-course',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './create-course.component.html',
  providers: [CreateCourseService] // ADDED SERVICE PROVIDER
})
export class CreateCourseComponent implements OnInit {
  step = 1;
  courseForm!: FormGroup;
  fileForm!: FormGroup;
  selectedThumbnail: File | null = null;
  thumbnailPreview: string | ArrayBuffer | null = null;
  loading$!: Observable<boolean>;

  tagInput = '';
  maxTags = 5;
  tags: string[] = [];

  domainOptions = ['AI', 'Web', 'Cloud', 'Security', 'Data Science'];
  levelOptions = ['Beginner', 'Intermediate', 'Advanced'];
  languageOptions = ['English', 'Hindi', 'Spanish'];

  constructor(
    private fb: FormBuilder,
    private instructorService: InstructorService,
    private createCourseService: CreateCourseService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loading$ = this.createCourseService.loading$;

    this.courseForm = this.fb.group({
      title: ['', [Validators.required, Validators.minLength(5)]],
      description: ['', [Validators.required, Validators.minLength(20)]],
      domain: ['', Validators.required],
      level: ['', Validators.required],
      language: ['', Validators.required],
      tags: [[]]
    });

    this.fileForm = this.fb.group({
      topic: ['', [Validators.required, Validators.minLength(3)]],
      description: ['', [Validators.required, Validators.minLength(10)]],
      file: [null, Validators.required]
    });
  }

  // FIXED TAG HANDLING
  addTag(event: KeyboardEvent): void {
    // Prevent form submission on Enter
    if (event.key === 'Enter') event.preventDefault();
    
    const input = this.tagInput.trim();
    if (input && !this.tags.includes(input)) {
      if (this.tags.length < this.maxTags) {
        this.tags.push(input);
        this.courseForm.get('tags')?.setValue([...this.tags]); // UPDATE FORM CONTROL
        this.tagInput = '';
      }
    }
  }

  removeTag(tag: string): void {
    this.tags = this.tags.filter(t => t !== tag);
    this.courseForm.get('tags')?.setValue([...this.tags]); // UPDATE FORM CONTROL
  }

  onThumbnailChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length) {
      this.selectedThumbnail = input.files[0];
      const reader = new FileReader();
      reader.onload = () => (this.thumbnailPreview = reader.result);
      reader.readAsDataURL(this.selectedThumbnail);
    }
  }

  // FIXED FILE UPLOAD HANDLING
  onFileChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length) {
      const file = input.files[0];
      this.fileForm.patchValue({ file });
      this.fileForm.get('file')?.updateValueAndValidity();
    }
  }

  submitCourse(): void {
    console.log('Submitting course...');
    
    // Debug form state
    console.log('Form valid:', this.courseForm.valid);
    console.log('Tags:', this.tags);
    console.log('Thumbnail:', !!this.selectedThumbnail);

    if (this.courseForm.invalid || !this.selectedThumbnail) {
      alert('Please complete all required fields and select a thumbnail');
      return;
    }

    this.createCourseService.setLoading(true);

    const formData = new FormData();
    formData.append('Title', this.courseForm.value.title);
    formData.append('Description', this.courseForm.value.description);
    formData.append('Domain', this.courseForm.value.domain);
    formData.append('Level', this.courseForm.value.level);
    formData.append('Language', this.courseForm.value.language);
    
    // Append tags properly
    this.tags.forEach(tag => formData.append('Tags', tag));
    
    formData.append('Thumbnail', this.selectedThumbnail);

    this.instructorService.addCourse(formData).subscribe({
      next: (res: any) => {
        if (res?.success && res?.data?.id) {
          this.createCourseService.setCourseId(res.data.id);
          this.step = 2;
        } else {
          alert('Course creation failed: ' + (res.message || 'Unknown error'));
        }
        this.createCourseService.setLoading(false);
      },
      error: err => {
        alert('Error: ' + (err.error?.message || err.message || 'Unknown error'));
        console.error(err);
        this.createCourseService.setLoading(false);
      },
    });
  }

  uploadContent(): void {
    console.log('Uploading content...');
    
    if (this.fileForm.invalid) {
      alert('Please complete all required fields');
      return;
    }

    const courseId = this.createCourseService.getCourseId();
    if (!courseId) {
      alert('Missing course ID');
      return;
    }

    const file = this.fileForm.get('file')?.value;
    if (!file) {
      alert('Please select a file');
      return;
    }

    const formData = new FormData();
    formData.append('File', file);
    formData.append('CourseId', courseId);
    formData.append('Topic', this.fileForm.value.topic);
    formData.append('Description', this.fileForm.value.description);

    this.createCourseService.setLoading(true);

    this.instructorService.uploadFile(formData).subscribe({
      next: res => {
        if (res?.success) {
          alert('Upload successful!');
          this.router.navigate(['/instructor-dashboard']);
        } else {
          alert('Upload failed: ' + (res.message || 'Unknown error'));
        }
        this.createCourseService.setLoading(false);
      },
      error: err => {
        alert('Error uploading: ' + (err.error?.message || err.message || 'Unknown error'));
        console.error(err);
        this.createCourseService.setLoading(false);
      },
    });
  }
}