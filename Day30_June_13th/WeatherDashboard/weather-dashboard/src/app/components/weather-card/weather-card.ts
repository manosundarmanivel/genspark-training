import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { WeatherData } from '../../services/weather';

@Component({
  selector: 'app-weather-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './weather-card.html',
  styleUrls: ['./weather-card.css']
})
export class WeatherCardComponent {
  @Input() weather!: WeatherData;

  get iconUrl(): string {
    return this.weather.icon ? `https://openweathermap.org/img/wn/${this.weather.icon}@2x.png` : '';
  }
}
