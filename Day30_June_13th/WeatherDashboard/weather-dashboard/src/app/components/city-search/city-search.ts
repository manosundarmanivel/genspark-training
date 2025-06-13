import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { WeatherService } from '../../services/weather';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-city-search',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './city-search.html',
  styleUrls: ['./city-search.css']
})
export class CitySearchComponent {
  city = '';

  constructor(public weatherService: WeatherService) {}

  search(cityFromHistory?: string) {
    const targetCity = cityFromHistory || this.city.trim();
    if (targetCity) {
      this.weatherService.fetchWeather(targetCity).subscribe();
      this.city = ''; // Clear the input after searching
    }
  }
}
