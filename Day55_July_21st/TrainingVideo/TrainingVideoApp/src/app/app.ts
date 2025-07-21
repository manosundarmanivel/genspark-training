import { Component, NgModule } from '@angular/core';

import { UploadVideoComponent } from './components/upload-video/upload-video.component';
import { VideoListComponent } from './components/video-list/video-list.component';

@Component({
  selector: 'app-root',
  imports: [UploadVideoComponent, VideoListComponent],
  templateUrl: './app.html',
  styleUrl: './app.css',


 
  
})
export class App {
  protected title = 'TrainingVideoApp';
}

