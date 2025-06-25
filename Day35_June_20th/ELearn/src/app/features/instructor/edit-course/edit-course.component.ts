import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { InstructorService } from '../services/instructor.service';

@Component({
  selector: 'app-edit-course',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './edit-course.component.html',
})
export class EditCourseComponent implements OnInit {
  courseId!: string;
  courseForm!: FormGroup;
  filesArray!: FormArray;
  newFileForm!: FormGroup;

domainOptions = ['Artificial Intelligence', 'Web Development', 'Data Science', 'Cybersecurity', 'Cloud Computing'];
  levelOptions = ['Beginner', 'Intermediate', 'Advanced'];
  languageOptions = ['English', 'Hindi', 'Spanish', 'French', 'German'];

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private instructorService: InstructorService
  ) {}

  ngOnInit(): void {
    this.courseId = this.route.snapshot.paramMap.get('id')!;
    console.log('Course ID from route:', this.courseId);

    this.courseForm = this.fb.group({
      title: ['', Validators.required],
      description: ['', Validators.required],
      domain: [''],
      level: [''],
      language: [''],
      tags: [[]],
      files: this.fb.array([]),
    });

    this.newFileForm = this.fb.group({
      topic: ['', Validators.required],
      description: ['', Validators.required],
      file: [null, Validators.required]
    });

    this.loadCourseData();
  }

  get filesFormArray(): FormArray<FormGroup> {
    return this.courseForm.get('files') as FormArray<FormGroup>;
  }

  private loadCourseData(): void {
    console.log('Loading course details...');
    this.instructorService.getCourseById(this.courseId).subscribe(course => {
      console.log('Fetched course data:', course);
      this.courseForm.patchValue(course);
    });

    console.log('Loading course files...');
    this.instructorService.getCourseFiles(this.courseId).subscribe((files) => {
      console.log('Fetched course files:', files);
      this.filesFormArray.clear();
      for (const file of files) {
        this.filesFormArray.push(this.fb.group({
          id: [file.id],
          topic: [file.topic, Validators.required],
          description: [file.description, Validators.required],
        }));
      }
    });
  }

  onUpdateFile(index: number): void {
    const fileGroup = this.filesFormArray.at(index) as FormGroup;
    const fileId = fileGroup.get('id')?.value;
    const updatePayload = {
      topic: fileGroup.get('topic')?.value,
      description: fileGroup.get('description')?.value,
    };

    console.log(`Updating file ID: ${fileId}`, updatePayload);
    this.instructorService.updateCourseFile(fileId, updatePayload).subscribe({
      next: () => {
        console.log('File updated successfully');
        alert('File updated');
      },
      error: (err) => {
        console.error('File update failed:', err);
        alert('Update failed: ' + err.message);
      }
    });
  }

  onNewFileChange(event: Event): void {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (file) {
      console.log('New file selected:', file);
      this.newFileForm.patchValue({ file });
      this.newFileForm.get('file')?.updateValueAndValidity();
    }
  }

  onAddNewFile(): void {
    if (this.newFileForm.invalid) {
      console.warn('New file form is invalid');
      return;
    }

    const formData = new FormData();
    const file = this.newFileForm.get('file')?.value;
    formData.append('File', file, file.name);
    formData.append('CourseId', this.courseId);
    formData.append('Topic', this.newFileForm.value.topic);
    formData.append('Description', this.newFileForm.value.description);

    console.log('Uploading new file:', {
      CourseId: this.courseId,
      Topic: this.newFileForm.value.topic,
      Description: this.newFileForm.value.description,
      File: file,
    });

    this.instructorService.uploadFile(formData).subscribe({
      next: () => {
        console.log('File uploaded successfully');
        alert('New file added');
        this.newFileForm.reset();
        this.loadCourseData();
      },
      error: (err) => {
        console.error('File upload failed:', err);
        alert('Upload failed: ' + err.message);
      }
    });
  }

  onUpdateCourse(): void {
    if (this.courseForm.invalid) {
      console.warn('Course form is invalid');
      return;
    }

    const payload = this.courseForm.value;
    console.log('Updating course with data:', payload);

    this.instructorService.updateCourse(this.courseId, payload).subscribe({
      next: () => {
        console.log('Course updated successfully');
        alert('Course updated successfully');
      },
      error: (err) => {
        console.error('Course update failed:', err);
        alert('Update failed: ' + err.message);
      }
    });
  }
}
