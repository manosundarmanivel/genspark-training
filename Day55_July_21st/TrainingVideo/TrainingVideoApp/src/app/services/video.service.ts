import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export interface VideoDto {
  id: number;
  title: string;
  description: string;
  uploadDate: string;
  blobUrl: string;
}

@Injectable({ providedIn: 'root' })
export class VideoService {
  public apiUrl = 'http://localhost:5261/api/videos';

  constructor(private http: HttpClient) {}

 uploadChunk(formData: FormData): Observable<any> {
  return this.http.post(`${this.apiUrl}/chunk-upload`, formData);
}


  getAllVideos(): Observable<VideoDto[]> {
    return this.http.get<VideoDto[]>(this.apiUrl);
  }

// streamVideo(id: number): Observable<Blob> {
//   return this.http.get(`${this.apiUrl}/stream/${id}`, {
//     responseType: 'blob'
//   });

  
// }

}
