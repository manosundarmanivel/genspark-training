import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-first',
  imports: [FormsModule], 
  templateUrl: './first.html',
  styleUrl: './first.css'
})

export class First {
  name:string;
    constructor(){
      this.name = "Ramu"
    }
    onButtonClick(uname:string){
      this.name = uname;
    }
  }

