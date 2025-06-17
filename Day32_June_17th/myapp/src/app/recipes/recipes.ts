
import { Component, OnInit, signal } from '@angular/core';
import { RecipeModel } from '../models/recipe';
import { RecipeService } from '../service/recipe.service';
import { Recipe } from '../recipe/recipe'; 

@Component({
  selector: 'app-recipes',
  standalone: true,
  imports: [Recipe],
  templateUrl: './recipes.html',
  styleUrls: ['./recipes.css'],
})
export class Recipes implements OnInit {
  recipes = signal<RecipeModel[] | null>(null);

  constructor(private recipeService: RecipeService) {}

  ngOnInit(): void {
    this.recipeService.getAllRecipes().subscribe({
      next: (data: any) => {
        this.recipes.set([]);
        console.log('Recipes fetched successfully:', this.recipes());
      },
      error: (err: any) => {
        console.error('Error fetching recipes:', err);
      },
      complete: () => {
        console.log('Recipe fetching completed');
      }
    });
  }
}
