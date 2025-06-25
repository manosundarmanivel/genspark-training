import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Recipe } from './recipe';
import { RecipeModel } from '../models/recipe';
import { By } from '@angular/platform-browser';
import { Component } from '@angular/core';

fdescribe('Recipe Component', () => {
  let component: Recipe;
  let fixture: ComponentFixture<Recipe>;

  const mockRecipe: RecipeModel = new RecipeModel(
    1,
    'Classic Margherita Pizza',
    'Italian',
    15,
    20,
    ['Pizza dough', 'Tomato sauce', 'Cheese'],
    'https://cdn.dummyjson.com/recipe-images/1.webp'
  );

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Recipe]
    }).compileComponents();

    fixture = TestBed.createComponent(Recipe);
    component = fixture.componentInstance;
    component.recipe = mockRecipe;
    fixture.detectChanges();
  });

  it('should create the Recipe component', () => {
    expect(component).toBeTruthy();
  });

  it('should display the recipe name', () => {
    const titleElement = fixture.nativeElement.querySelector('.card-title');
    expect(titleElement.textContent).toContain(mockRecipe.name);
  });

  it('should display the correct cuisine', () => {
    const cuisineElement = fixture.nativeElement.querySelector('.recipe-meta li');
    expect(cuisineElement.textContent).toContain(mockRecipe.cuisine);
  });

  it('should display total time (prep + cook)', () => {
    const timeElement = fixture.nativeElement.querySelectorAll('.recipe-meta li')[1];
    const totalTime = mockRecipe.prepTimeMinutes + mockRecipe.cookTimeMinutes;
    expect(timeElement.textContent).toContain(`${totalTime} mins`);
  });

  it('should display the ingredients list', () => {
    const ingredientsElement = fixture.nativeElement.querySelector('.card-text');
    for (const ingredient of mockRecipe.ingredients) {
      expect(ingredientsElement.textContent).toContain(ingredient);
    }
  });

  it('should bind the image source and alt text', () => {
    const imgElement = fixture.debugElement.query(By.css('img')).nativeElement as HTMLImageElement;
    expect(imgElement.src).toBe(mockRecipe.image);
    expect(imgElement.alt).toBe(mockRecipe.name);
  });
});
