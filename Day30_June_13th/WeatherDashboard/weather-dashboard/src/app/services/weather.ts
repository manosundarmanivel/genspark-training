import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, ReplaySubject, interval, of } from 'rxjs';
import { catchError, switchMap, tap } from 'rxjs/operators';

export interface WeatherData {
  city: string;
  temp: number;
  condition: string;
  icon: string;
  humidity: number;
  windSpeed: number;
  error?: string;
}

@Injectable({
  providedIn: 'root'
})
export class WeatherService {
  private apiKey = '968df098cfdf94084c866bf7597cc025';
  private apiUrl = 'https://api.openweathermap.org/data/2.5/weather';

  private weatherSubject = new BehaviorSubject<WeatherData | null>(null);
  weather$ = this.weatherSubject.asObservable();

  private history: string[] = JSON.parse(localStorage.getItem('weatherHistory') || '[]');
  private historySubject = new ReplaySubject<string[]>(1);
  history$ = this.historySubject.asObservable();

  private currentCity = '';

  constructor(private http: HttpClient) {
    this.historySubject.next(this.history);

   
    interval(300000).pipe(
      switchMap(() => this.currentCity ? this.fetchWeather(this.currentCity, false) : of(null))
    ).subscribe();
  }

  fetchWeather(city: string, addToHistory: boolean = true) {
    this.currentCity = city;

    return this.http.get<any>(`${this.apiUrl}?q=${city}&units=metric&appid=${this.apiKey}`).pipe(
      tap(response => {
        const data: WeatherData = {
          city: response.name,
          temp: response.main.temp,
          condition: response.weather[0].description,
          icon: response.weather[0].icon,
          humidity: response.main.humidity,
          windSpeed: response.wind.speed
        };
        this.weatherSubject.next(data);

        if (addToHistory) {
          this.updateHistory(city);
        }
      }),
      catchError(error => {
        this.weatherSubject.next({
          city,
          temp: 0,
          condition: '',
          icon: '',
          humidity: 0,
          windSpeed: 0,
          error: error?.error?.message || 'City not found or network error'
        });
        return of(null);
      })
    );
  }

  private updateHistory(city: string) {
    this.history = [city, ...this.history.filter(c => c.toLowerCase() !== city.toLowerCase())].slice(0, 5);
    localStorage.setItem('weatherHistory', JSON.stringify(this.history));
    this.historySubject.next(this.history);
  }
}
