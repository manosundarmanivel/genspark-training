import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  private http = inject(HttpClient);

  getAllProducts(): Observable<any> {
    return this.http.get<any>('https://dummyjson.com/products');
  }

getSearchProducts(searchInput: string, skip = 0): Observable<any> {
  return this.http.get<any>(
    `https://dummyjson.com/products/search?q=${encodeURIComponent(searchInput)}&skip=${skip}&limit=10`
  );
}

}
