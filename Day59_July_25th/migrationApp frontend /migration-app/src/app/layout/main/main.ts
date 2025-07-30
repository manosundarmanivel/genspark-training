import { Component } from '@angular/core';
import { RouterModule, RouterOutlet } from '@angular/router';
import { Category, CategoryService } from '../../pages/category/category.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-main',
  standalone: true,
  imports: [RouterOutlet, RouterModule, CommonModule],
  templateUrl: './main.html',
  styleUrls: ['./main.css']
})
export class Main {

   categories: Category[] = [];

   constructor(private categoryService: CategoryService) {}
     ngOnInit(): void {
    this.loadCategories();
  }

    loadCategories() {
    this.categoryService.getAll().subscribe(data => {
      this.categories = data;
    });
  }
}
