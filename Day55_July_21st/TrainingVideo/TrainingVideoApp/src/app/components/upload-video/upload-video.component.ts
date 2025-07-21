import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { VideoService } from '../../services/video.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-upload-video',
  templateUrl: './upload-video.component.html',
  imports: [ReactiveFormsModule, CommonModule]
})
export class UploadVideoComponent {
  form: FormGroup;
  uploading = false;
  uploadProgress = 0; // add this to the class


  constructor(private fb: FormBuilder, private videoService: VideoService) {
    this.form = this.fb.group({
      title: [''],
      description: [''],
      file: [null]
    });
  }

  onFileChange(event: any) {
    if (event.target.files.length > 0) {
      this.form.patchValue({ file: event.target.files[0] });
    }
  }

upload() {
  const file: File = this.form.get('file')?.value;
  const title = this.form.get('title')?.value;
  const description = this.form.get('description')?.value;

  if (!file) {
    alert("Please select a video file.");
    return;
  }

  const chunkSize = 2 * 1024 * 1024; // 2MB
  const totalChunks = Math.ceil(file.size / chunkSize);
  const fileId = crypto.randomUUID();

  this.uploading = true;

const uploadChunk = (index: number) => {
  const start = index * chunkSize;
  const end = Math.min(start + chunkSize, file.size);
  const chunk = file.slice(start, end);

  const formData = new FormData();
  formData.append('chunk', chunk);
  formData.append('fileId', fileId);
  formData.append('fileName', file.name);
  formData.append('chunkIndex', index.toString());
  formData.append('totalChunks', totalChunks.toString());
  formData.append('title', title);
  formData.append('description', description);

  this.videoService.uploadChunk(formData).subscribe({
    next: () => {
      this.uploadProgress = Math.round(((index + 1) / totalChunks) * 100);
      if (index + 1 < totalChunks) {
        uploadChunk(index + 1);
      } else {
        alert('Upload completed!');
        this.form.reset();
        this.uploading = false;
        this.uploadProgress = 0;
      }
    },
    error: () => {
      alert(`Upload failed at chunk ${index + 1}`);
      this.uploading = false;
    }
  });
};


  uploadChunk(0);
}

}
