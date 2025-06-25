import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { RecipeService } from './recipe.service';

fdescribe('RecipeService', () => {
  let service: RecipeService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule], 
      
    });

    service = TestBed.inject(RecipeService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify(); 
  });

  it('should fetch all recipes and map only used properties', () => {
    const mockResponse = {
      recipes: [
        {
          id: 1,
          name: 'Classic Margherita Pizza',
          cuisine: 'Italian',
          cookTimeMinutes: 15,
          prepTimeMinutes: 20,
          ingredients: ['Pizza dough', 'Tomato sauce'],
          image: 'https://cdn.dummyjson.com/recipe-images/1.webp'
        }
      ]
    };

    service.getAllRecipes().subscribe(response => {
      expect(response.recipes.length).toBe(1);
      expect(response.recipes[0].name).toBe('Classic Margherita Pizza');
    });

    const req = httpMock.expectOne('https://dummyjson.com/recipes');
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });
});
