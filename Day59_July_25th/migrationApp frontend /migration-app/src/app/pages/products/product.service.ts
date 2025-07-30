import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ProductDto {
  productId: number;
  productName: string;
  image: string;
  price?: number;
  userId?: number;
  categoryId?: number;
  colorId?: number;
  modelId?: number;
  quantity?: string;
  sellStartDate?: string;
  sellEndDate?: string;
  isNew?: number;
}

export interface CreateProductDto {
  productName: string;
  imageFile: File | null;
  price?: number;
  userId?: number;
  categoryId?: number;
  colorId?: number;
  modelId?: number;
  sellStartDate?: string;
  sellEndDate?: string;
  isNew?: number;
}

export interface UpdateProductDto {
  productId: number;
  productName: string;
  image?: string;
  price?: number;
  userId?: number;
  categoryId?: number;
  colorId?: number;
  modelId?: number;
  sellStartDate?: string;
  sellEndDate?: string;
  isNew?: number;
}


@Injectable({ providedIn: 'root' })
export class ProductService {
  private base = 'http://localhost:5228/api/products';

  constructor(private http: HttpClient) {}

  getAll(): Observable<ProductDto[]> { return this.http.get<ProductDto[]>(this.base); }
  getById(id: number): Observable<ProductDto> { return this.http.get<ProductDto>(`${this.base}/${id}`); }

  create(formData: FormData): Observable<ProductDto> {
    return this.http.post<ProductDto>(this.base, formData);
  }
  update(id: number, dto: UpdateProductDto): Observable<ProductDto> {
    return this.http.put<ProductDto>(`${this.base}/${id}`, dto);
  }
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`);
  }
}
