import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface NewsDto {
  newsId: number;
  userId?: number;
  title: string;
  shortDescription?: string;
  image?: string; // This is the path returned from backend
  content?: string;
  createdDate?: Date;
  status?: number;
}

export interface CreateNewsDto {
  userId?: number;
  title: string;
  shortDescription?: string;
  imageFile: File | null;
  content?: string;
  createdDate?: Date;
  status?: number;
}

@Injectable({ providedIn: 'root' })
export class NewsService {
  private apiUrl = 'http://localhost:5228/api/news';

  constructor(private http: HttpClient) {}

  getAll(): Observable<NewsDto[]> {
    return this.http.get<NewsDto[]>(this.apiUrl);
  }

  getById(id: number): Observable<NewsDto> {
    return this.http.get<NewsDto>(`${this.apiUrl}/${id}`);
  }

  create(dto: CreateNewsDto): Observable<NewsDto> {
    const formData = this.toFormData(dto);
    return this.http.post<NewsDto>(this.apiUrl, formData);
  }

  update(id: number, dto: CreateNewsDto): Observable<void> {
    const formData = this.toFormData(dto);
    return this.http.put<void>(`${this.apiUrl}/${id}`, formData);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  private toFormData(dto: CreateNewsDto): FormData {
    const formData = new FormData();
    if (dto.userId !== undefined) formData.append('userId', dto.userId.toString());
    formData.append('title', dto.title);
    if (dto.shortDescription) formData.append('shortDescription', dto.shortDescription);
    if (dto.content) formData.append('content', dto.content);
    if (dto.createdDate) formData.append('createdDate', dto.createdDate.toISOString());
    if (dto.status !== undefined) formData.append('status', dto.status.toString());
    if (dto.imageFile) formData.append('imageFile', dto.imageFile);
    return formData;
  }
}
