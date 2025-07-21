import { Component, OnInit } from '@angular/core';
import { VideoDto, VideoService } from '../../services/video.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UploadVideoComponent } from "../upload-video/upload-video.component";

@Component({
  selector: 'app-video-list',
  templateUrl: './video-list.component.html',
  standalone: true,
  imports: [CommonModule, FormsModule, UploadVideoComponent]
})
export class VideoListComponent implements OnInit {
  videos: VideoDto[] = [];
  searchTerm = '';
  selectedVideoUrl: string | null = null;
  showUpload = false;

  constructor(private videoService: VideoService) {}

  ngOnInit() {
    this.videoService.getAllVideos().subscribe(v => (this.videos = v));
  }

  get filteredVideos(): VideoDto[] {
    if (!this.searchTerm.trim()) return this.videos;

    const term = this.searchTerm.toLowerCase();
    return this.videos.filter(
      video =>
        video.title.toLowerCase().includes(term) ||
        video.description.toLowerCase().includes(term)
    );
  }

  play(video: VideoDto) {
    this.selectedVideoUrl = `${this.videoService.apiUrl}/stream/${video.id}`;
  }
}
