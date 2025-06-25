import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { Recipes } from './recipes';
import { RecipeService } from '../service/recipe.service';
import { of, throwError } from 'rxjs';
import { RecipeModel } from '../models/recipe';
import { Recipe } from '../recipe/recipe';
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

fdescribe('Recipes Component', () => {
  let component: Recipes;
  let fixture: ComponentFixture<Recipes>;
  let mockRecipeService: jasmine.SpyObj<RecipeService>;

  const mockRecipes: RecipeModel[] = [
    {
      id: 1,
      name: 'Test Recipe 1',
      cuisine: 'Italian',
      cookTimeMinutes: 20,
      prepTimeMinutes: 10,
      ingredients: ['Cheese', 'Tomato'],
      image: 'https://cdn.dummyjson.com/recipe-images/1.webp',
    },
    {
      id: 2,
      name: 'Test Recipe 2',
      cuisine: 'Indian',
      cookTimeMinutes: 30,
      prepTimeMinutes: 15,
      ingredients: ['Spices', 'Rice'],
      image: 'https://cdn.dummyjson.com/recipe-images/2.webp',
    }
  ];

  beforeEach(waitForAsync(() => {
    mockRecipeService = jasmine.createSpyObj('RecipeService', ['getAllRecipes']);

    TestBed.configureTestingModule({
      imports: [Recipes, Recipe, CommonModule],
      providers: [
        { provide: RecipeService, useValue: mockRecipeService }
      ]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(Recipes);
    component = fixture.componentInstance;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should display a list of recipes when data is available', () => {
    mockRecipeService.getAllRecipes.and.returnValue(of({ recipes: mockRecipes }));

    fixture.detectChanges();

    const recipeCards = fixture.nativeElement.querySelectorAll('app-recipe');
    expect(recipeCards.length).toBe(2);
  });

  it('should display "No recipes available" when the list is empty', () => {
    mockRecipeService.getAllRecipes.and.returnValue(of({ recipes: [] }));

    fixture.detectChanges();

    const noRecipesMsg = fixture.nativeElement.querySelector('.no-recipes');
    expect(noRecipesMsg).toBeTruthy();
    expect(noRecipesMsg.textContent).toContain('No recipes available');
  });

  it('should handle API error and log it', () => {
    spyOn(console, 'error');
    mockRecipeService.getAllRecipes.and.returnValue(throwError(() => new Error('API failure')));

    fixture.detectChanges();

    expect(console.error).toHaveBeenCalledWith('Error fetching recipes:', jasmine.any(Error));

    const noRecipesMsg = fixture.nativeElement.querySelector('.no-recipes');
    expect(noRecipesMsg).toBeTruthy();
    expect(noRecipesMsg.textContent).toContain('No recipes available');
  });
});
