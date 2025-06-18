
import { HttpClient } from '@angular/common/http';
import { inject } from '@angular/core';
import { Observable } from 'rxjs';
import { RecipeModel } from '../models/recipe';

export class RecipeService {
  private http = inject(HttpClient);

  getAllRecipes(): Observable<{ recipes: RecipeModel[] }> {
    return this.http.get<{ recipes: RecipeModel[] }>('https://dummyjson.com/recipes');
  }
}
