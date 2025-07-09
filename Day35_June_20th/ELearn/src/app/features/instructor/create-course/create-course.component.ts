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
import { BehaviorSubject, Observable } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { ChangeDetectorRef } from '@angular/core';



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
    private thumbnailPreviewSubject = new BehaviorSubject<string | ArrayBuffer | null>(null);
  thumbnailPreview$: Observable<string | ArrayBuffer | null> = this.thumbnailPreviewSubject.asObservable();
    loading$!: Observable<boolean>;

    tagInput = '';
    maxTags = 5;
    tags: string[] = [];


    domainOptions = ['Artificial Intelligence', 'Web Development', 'Data Science', 'Cybersecurity', 'Cloud Computing'];
    levelOptions = ['Beginner', 'Intermediate', 'Advanced'];
    languageOptions = ['English', 'Hindi', 'Spanish', 'French', 'German'];

    uploadProgress = 0;

    accessToken = localStorage.getItem('token');


    constructor(
        private fb: FormBuilder,
        private instructorService: InstructorService,
        private createCourseService: CreateCourseService,
        private router: Router,
        private toastr: ToastrService,
        private cdr: ChangeDetectorRef 
    ) { 

       
       
    }

    ngOnInit(): void {
        this.loading$ = this.createCourseService.loading$;

        this.courseForm = this.fb.group({
            title: ['', [Validators.required, Validators.minLength(5), Validators.maxLength(40)] ],
            description: ['', [Validators.required, Validators.minLength(20)]],
            domain: ['', Validators.required],
            level: ['', Validators.required],
            language: ['', Validators.required],
            price:['', Validators.required],
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
    reader.onload = () => {
      this.thumbnailPreviewSubject.next(reader.result); // update observable
    };
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
            this.toastr.error('Please complete all required fields and select a thumbnail');
            return;
        }

        this.createCourseService.setLoading(true);

        const formData = new FormData();
        formData.append('Title', this.courseForm.value.title);
        formData.append('Description', this.courseForm.value.description);
        formData.append('Domain', this.courseForm.value.domain);
        formData.append('Level', this.courseForm.value.level);
        formData.append('Language', this.courseForm.value.language);
        formData.append('Price', this.courseForm.value.price)

        // Append tags properly
        this.tags.forEach(tag => formData.append('Tags', tag));

        formData.append('Thumbnail', this.selectedThumbnail);

        this.instructorService.addCourse(formData).subscribe({
            next: (res: any) => {
                if (res?.success && res?.data?.id) {
                    this.createCourseService.setCourseId(res.data.id);
                    this.step = 2;
                } else {
                    this.toastr.error('Course creation failed: ' + (res.message || 'Unknown error'));
                }
                this.createCourseService.setLoading(false);
            },
            error: err => {
               this.toastr.error('Error: ' + (err.error?.message || err.message || 'Unknown error'));
                console.error(err);
                this.createCourseService.setLoading(false);
            },
        });
    }

 


    uploadContent(): void {




        if (this.fileForm.invalid) {
            this.toastr.warning('Please complete all required fields');
            this.cdr.detectChanges(); 
            return;
        }

        const courseId = this.createCourseService.getCourseId();
        const file: File = this.fileForm.get('file')?.value;
        if (!courseId || !file) return;

        this.createCourseService.setLoading(true);

        const worker = new Worker(
            new URL('../worker/upload.worker.ts', import.meta.url),
            { type: 'module' }
        );

        worker.postMessage({
            file,
            courseId,
            topic: this.fileForm.value.topic,
            description: this.fileForm.value.description,
            chunkSize: 1 * 1024 * 1024,
            uploadUrl: 'http://localhost:5295/api/v1/files/upload/chunk',
            token: this.accessToken
        });

        worker.onmessage = ({ data }) => {
            if (data.progress !== undefined) {
                this.uploadProgress = data.progress;
            }

            if (data.error) {
                this.toastr.error('Upload failed: ' + data.error);
                this.cdr.detectChanges(); 
                this.createCourseService.setLoading(false);
                worker.terminate();
            }

            if (data.done) {
                this.uploadProgress = 100;
                this.toastr.success('Upload complete!');
                 this.cdr.detectChanges(); 
                this.createCourseService.setLoading(false);
                this.router.navigate(['/instructor-dashboard']);
                worker.terminate();
            }
        };

    }

}