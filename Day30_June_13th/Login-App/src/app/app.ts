import { Component } from '@angular/core';

import { CommonModule } from '@angular/common';
import { NavbarComponent } from "./navbar.component";
import { LoginComponent } from "./login.component";





@Component({
  selector: 'app-root',

  templateUrl: './app.html',
  styleUrl: './app.css',
  imports: [NavbarComponent, LoginComponent],
  
})


export class App {
  protected title = 'myapp';


  
}
