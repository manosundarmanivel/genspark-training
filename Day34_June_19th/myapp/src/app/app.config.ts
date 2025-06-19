import { ApplicationConfig, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient } from '@angular/common/http';
import { ProductService } from './service/product.service';
import { RecipeService } from './service/recipe.service';
import { provideCharts, withDefaultRegisterables } from 'ng2-charts';
import { provideStore, StoreModule } from '@ngrx/store';
import { EffectsModule, provideEffects } from '@ngrx/effects';
import { userReducer } from '../app/ngrx/userReducer';



export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(),
   
    ProductService,
RecipeService, provideCharts(withDefaultRegisterables()),
provideStore({ userState: userReducer }),



  ]
};
