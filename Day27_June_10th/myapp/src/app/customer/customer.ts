import { Component } from '@angular/core';
import { CustomerModel } from './customer.model';

@Component({
  selector: 'app-customer',
  imports: [],
  templateUrl: './customer.html',
  styleUrl: './customer.css'
})
export class Customer {

  customer: CustomerModel = {
    name: 'Mano Sundar',
    email: 'mano@example.com',
    city: 'Chennai'
  };

  likes = 0;
  dislikes = 0;

  like() {
    this.likes++;
  }

  dislike() {
    this.dislikes++;
  }
}
