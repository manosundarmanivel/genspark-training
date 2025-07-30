import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ColorService, Color, CreateColorDto, UpdateColorDto } from './color.service';

@Component({
  selector: 'app-color',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './color.html',
  styleUrls: ['./color.css']
})
export class ColorComponent {
  colors: Color[] = [];
  currentPage = 1;
  pageSize = 5;

  mode: 'list' | 'create' | 'edit' | 'details' = 'list';

  form: Partial<Color> = {};
  selectedColor: Color | null = null;

  constructor(private colorService: ColorService) {}

  ngOnInit(): void {
    this.loadColors();
  }

  loadColors() {
    this.colorService.getAll().subscribe(data => this.colors = data);
  }

  get pagedColors(): Color[] {
    const start = (this.currentPage - 1) * this.pageSize;
    return this.colors.slice(start, start + this.pageSize);
  }

  nextPage() {
    if (this.currentPage * this.pageSize < this.colors.length) {
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

  showEdit(color: Color) {
    this.form = { ...color };
    this.mode = 'edit';
  }

  showDetails(color: Color) {
    this.selectedColor = color;
    this.mode = 'details';
  }

  cancel() {
    this.mode = 'list';
    this.form = {};
    this.selectedColor = null;
  }

  createColor() {
    const dto: CreateColorDto = { colorName: this.form.colorName! };
    this.colorService.create(dto).subscribe(() => {
      this.loadColors();
      this.cancel();
    });
  }

  updateColor() {
    const dto: UpdateColorDto = {
      colorId: this.form.colorId!,
      colorName: this.form.colorName!
    };
    this.colorService.update(dto.colorId, dto).subscribe(() => {
      this.loadColors();
      this.cancel();
    });
  }

  deleteColor(id: number) {
    if (confirm('Are you sure you want to delete this color?')) {
      this.colorService.delete(id).subscribe(() => this.loadColors());
    }
  }
}
