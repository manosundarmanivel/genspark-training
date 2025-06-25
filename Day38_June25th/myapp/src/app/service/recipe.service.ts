import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RecipeModel } from '../models/recipe';

@Injectable({ providedIn: 'root' }) // ✅ Make it injectable
export class RecipeService {
  private http = inject(HttpClient);

  getAllRecipes(): Observable<{ recipes: RecipeModel[] }> {
    return this.http.get<{ recipes: RecipeModel[] }>('https://dummyjson.com/recipes');
  }
}
