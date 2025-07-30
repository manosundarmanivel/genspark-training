import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NewsService, NewsDto, CreateNewsDto } from './news.service';

@Component({
  selector: 'app-news',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './news.html',
  styleUrls: ['./news.css']
})
export class NewsComponent {
  newsList: NewsDto[] = [];
  currentPage = 1;
  pageSize = 5;

  mode: 'list' | 'create' | 'edit' | 'details' = 'list';
  form: Partial<CreateNewsDto> & { newsId?: number } = {};
  selectedNews: NewsDto | null = null;
  selectedFile: File | null = null;

  constructor(private newsService: NewsService) {}

  ngOnInit(): void {
    this.loadNews();
  }

  loadNews() {
    this.newsService.getAll().subscribe(data => this.newsList = data);
  }

  get pagedNews(): NewsDto[] {
    const start = (this.currentPage - 1) * this.pageSize;
    return this.newsList.slice(start, start + this.pageSize);
  }

  nextPage() {
    if (this.currentPage * this.pageSize < this.newsList.length) this.currentPage++;
  }

  prevPage() {
    if (this.currentPage > 1) this.currentPage--;
  }

  showCreate() {
    this.form = {};
    this.selectedFile = null;
    this.mode = 'create';
  }

  showEdit(news: NewsDto) {
    this.form = {
      userId: news.userId,
      title: news.title,
      shortDescription: news.shortDescription,
      content: news.content,
      createdDate: news.createdDate,
      status: news.status
    };
    this.selectedFile = null;
    this.form['newsId'] = news.newsId; // preserve id for update
    this.mode = 'edit';
  }

  showDetails(news: NewsDto) {
    this.selectedNews = news;
    this.mode = 'details';
  }

  cancel() {
    this.mode = 'list';
    this.form = {};
    this.selectedNews = null;
    this.selectedFile = null;
  }

  onFileSelected(event: any) {
    this.selectedFile = event.target.files[0] ?? null;
  }

  createNews() {
    const dto: CreateNewsDto = {
      userId: this.form.userId,
      title: this.form.title!,
      shortDescription: this.form.shortDescription,
      content: this.form.content,
      createdDate: new Date(),
      status: this.form.status ?? 1,
      imageFile: this.selectedFile
    };

    this.newsService.create(dto).subscribe(() => {
      this.loadNews();
      this.cancel();
    });
  }

  updateNews() {
    if (!this.form.newsId) return;

    const dto: CreateNewsDto = {
      userId: this.form.userId,
      title: this.form.title!,
      shortDescription: this.form.shortDescription,
      content: this.form.content,
      createdDate: this.form.createdDate ?? new Date(),
      status: this.form.status ?? 1,
      imageFile: this.selectedFile
    };

    this.newsService.update(this.form.newsId, dto).subscribe(() => {
      this.loadNews();
      this.cancel();
    });
  }

  deleteNews(id: number) {
    if (confirm('Are you sure you want to delete this news?')) {
      this.newsService.delete(id).subscribe(() => this.loadNews());
    }
  }
}
