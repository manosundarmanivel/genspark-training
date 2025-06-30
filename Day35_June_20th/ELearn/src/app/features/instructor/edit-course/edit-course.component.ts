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
  this.instructorService.getCourseByIdEdit(this.courseId).subscribe(course => {
    console.log('Fetched course data:', course);


    
    this.courseForm.patchValue({
      title: course.title,
      description: course.description,
      domain: course.domain,
      level: course.level,
      language: course.language,
      tags: course.tags
    });

  
    const uploadedFiles = course.uploadedFiles || [];
    this.filesFormArray.clear();
   for (const file of uploadedFiles) {
  this.filesFormArray.push(this.fb.group({
    id: [file.id],
    topic: [file.topic, Validators.required],
    description: [file.description, Validators.required],
    path: [file.path],
    file: [null] // for updating file
  }));
}

  });
}



onUpdateFile(index: number): void {
  const fileGroup = this.filesFormArray.at(index) as FormGroup;
  const fileId = fileGroup.get('id')?.value;
  const file = fileGroup.get('file')?.value;

  if (!fileId) {
    alert('File ID is missing.');
    return;
  }

  const formData = new FormData();
  formData.append('topic', fileGroup.get('topic')?.value);
  formData.append('description', fileGroup.get('description')?.value);
  formData.append('courseId', this.courseId);

  if (file) {
    formData.append('file', file);
  }

  this.instructorService.updateCourseFile(fileId, formData).subscribe({
    next: () => {
      console.log('File updated successfully');
      alert('File updated');
      this.loadCourseData(); // reload updated info
    },
    error: (err) => {
      console.error('File update failed:', err);
      alert('Update failed: ' + err.message);
    }
  });
}


  onFileChange(event: Event, index: number): void {
  const file = (event.target as HTMLInputElement).files?.[0];
  if (file) {
    this.filesFormArray.at(index).patchValue({ file });
    this.filesFormArray.at(index).get('file')?.updateValueAndValidity();
  }
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
    alert('Please complete all required fields');
    return;
  }

  const file = this.newFileForm.get('file')?.value;
  const topic = this.newFileForm.value.topic;
  const description = this.newFileForm.value.description;

  if (!this.courseId || !file) {
    alert('Course ID or file is missing');
    return;
  }

  const uploadUrl = 'http://localhost:5295/api/v1/files/upload/chunk'; 
  const token = localStorage.getItem('token'); 

  const chunkSize = 1024 * 1024; // 1MB

  const worker = new Worker(
    new URL('../worker/upload.worker.ts', import.meta.url),
    { type: 'module' }
  );

  worker.postMessage({
    file,
    courseId: this.courseId,
    topic,
    description,
    chunkSize,
    uploadUrl,
    token
  });

  worker.onmessage = ({ data }) => {
    if (data.progress !== undefined) {
      console.log(`Upload progress: ${data.progress}%`);
    } else if (data.done) {
      console.log('Upload complete via worker');
      alert('New file added successfully!');
      this.newFileForm.reset();
      this.loadCourseData(); // refresh UI
      worker.terminate();
    } else if (data.error) {
      console.error('Worker upload error:', data.error);
      alert(`Upload failed: ${data.error}`);
      worker.terminate();
    }
  };

  worker.onerror = (err) => {
    console.error('Worker encountered an error:', err);
    alert('Something went wrong during upload.');
    worker.terminate();
  };
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
