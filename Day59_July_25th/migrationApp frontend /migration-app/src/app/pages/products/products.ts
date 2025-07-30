import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProductService } from './product.service';
import { CategoryService, Category } from '../category/category.service';
import { ColorService, Color } from '../color/color.service';
import { ProductDto, CreateProductDto, UpdateProductDto } from './product.service';

@Component({
  selector: 'app-product',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './products.html',
  styleUrls: ['./products.css']
})
export class ProductComponent {
  products: ProductDto[] = [];
  categories: Category[] = [];
  colors: Color[] = [];
  mode: 'list' | 'create' | 'edit' | 'details' = 'list';
  form: Partial<ProductDto> & { imageFile?: File } = {};
  selectedProduct: ProductDto | null = null;

  constructor(
    private svc: ProductService,
    private catSvc: CategoryService,
    private colorSvc: ColorService
  ) {}

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.svc.getAll().subscribe(p => this.products = p);
    this.catSvc.getAll().subscribe(c => this.categories = c);
    this.colorSvc.getAll().subscribe(c => this.colors = c);
  }

  showCreate() {
    this.form = {};
    this.mode = 'create';
  }

  showEdit(prod: ProductDto) {
    this.form = { ...prod };
    this.mode = 'edit';
  }

  showDetails(prod: ProductDto) {
    this.selectedProduct = prod;
    this.mode = 'details';
  }

  cancel() {
    this.mode = 'list';
    this.form = {};
    this.selectedProduct = null;
  }

  onFileChange(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files?.length) {
      this.form.imageFile = input.files[0];
    }
  }

  create() {
    const fd = new FormData();
    fd.append('ProductName', this.form.productName!);
    this.form.price != null && fd.append('Price', this.form.price!.toString());
    this.form.categoryId != null && fd.append('CategoryId', this.form.categoryId!.toString());
    this.form.colorId != null && fd.append('ColorId', this.form.colorId!.toString());
    if (this.form.imageFile) fd.append('ImageFile', this.form.imageFile);
    this.svc.create(fd).subscribe(() => this.loadData(), () => {}, () => this.cancel());
  }

  update() {
    const dto: UpdateProductDto = {
      productId: this.form.productId!,
      productName: this.form.productName!,
      price: this.form.price,
      image: this.form.image,
      categoryId: this.form.categoryId,
      colorId: this.form.colorId
    };
    this.svc.update(dto.productId, dto).subscribe(() => this.loadData(), () => {}, () => this.cancel());
  }

  delete(id: number) {
    if(confirm('Delete product?')) this.svc.delete(id).subscribe(() => this.loadData());
  }

  getCategoryName(categoryId?: number): string {
  if (!categoryId) return 'N/A';
  const cat = this.categories.find(c => c.categoryId === categoryId);
  return cat ? cat.name : 'Unknown';
}

getColorName(colorId?: number): string {
  if (!colorId) return 'N/A';
  const color = this.colors.find(c => c.colorId === colorId);
  return color ? color.colorName : 'Unknown';
}

}
