import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CategoryService, Category, CreateCategoryDto, UpdateCategoryDto } from './category.service';

@Component({
  selector: 'app-category',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './category.html',
  styleUrls: ['./category.css'],
})
export class CategoryComponent {
  categories: Category[] = [];
  currentPage = 1;
  pageSize = 2;

  mode: 'list' | 'create' | 'edit' | 'details' = 'list';


  form: Partial<Category> = {};
  selectedCategory: Category | null = null;

  constructor(private categoryService: CategoryService) {}

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories() {
    this.categoryService.getAll().subscribe(data => {
      this.categories = data;
    });
  }

  get pagedCategories(): Category[] {
    const start = (this.currentPage - 1) * this.pageSize;
    return this.categories.slice(start, start + this.pageSize);
  }

  nextPage() {
    if (this.currentPage * this.pageSize < this.categories.length) {
      this.currentPage++;
    }
  }

  prevPage() {
    if (this.currentPage > 1) {
      this.currentPage--;
    }
  }

  showCreate() {
    this.form = {};
    this.mode = 'create';
  }

  showEdit(category: Category) {
    this.form = { ...category };
    this.mode = 'edit';
  }

  showDetails(category: Category) {
    this.selectedCategory = category;
    this.mode = 'details';
  }

  cancel() {
    this.mode = 'list';
    this.form = {};
    this.selectedCategory = null;
  }

  createCategory() {
    const dto: CreateCategoryDto = { name: this.form.name! };
    this.categoryService.create(dto).subscribe(() => {
      this.loadCategories();
      this.cancel();
    });
  }

  updateCategory() {
    const dto: UpdateCategoryDto = {
      categoryId: this.form.categoryId!,
      name: this.form.name!,
    };
    this.categoryService.update(dto.categoryId, dto).subscribe(() => {
      this.loadCategories();
      this.cancel();
    });
  }

  deleteCategory(id: number) {
    if (confirm('Are you sure you want to delete this category?')) {
      this.categoryService.delete(id).subscribe(() => {
        this.loadCategories();
      });
    }
  }
}
