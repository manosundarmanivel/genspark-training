import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Color {
  colorId: number;
  colorName: string;
}

export interface CreateColorDto {
  colorName: string;
}

export interface UpdateColorDto {
  colorId: number;
  colorName: string;
}

@Injectable({ providedIn: 'root' })
export class ColorService {
  private apiUrl = 'http://localhost:5228/api/colors';

  constructor(private http: HttpClient) {}

  getAll(): Observable<Color[]> {
    return this.http.get<Color[]>(this.apiUrl);
  }

  getById(id: number): Observable<Color> {
    return this.http.get<Color>(`${this.apiUrl}/${id}`);
  }

  create(dto: CreateColorDto): Observable<Color> {
    return this.http.post<Color>(this.apiUrl, dto);
  }

  update(id: number, dto: UpdateColorDto): Observable<Color> {
    return this.http.put<Color>(`${this.apiUrl}/${id}`, dto);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
