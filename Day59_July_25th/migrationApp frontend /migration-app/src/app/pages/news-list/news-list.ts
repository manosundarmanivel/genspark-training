import { Component, OnInit } from '@angular/core';
import { NewsDto, NewsService } from '../news/news.service';
import { CommonModule } from '@angular/common';
import * as XLSX from 'xlsx';
import { saveAs } from 'file-saver';
import jsPDF from 'jspdf';
import autoTable from 'jspdf-autotable';

@Component({
  selector: 'app-news-list',
  templateUrl: './news-list.html',
  styleUrls: ['./news-list.css'],
  standalone: true,
  imports: [CommonModule]
})
export class NewsListComponent implements OnInit {
  newsList: NewsDto[] = [];
  loading = true;
  error = '';

  constructor(private newsService: NewsService) {}

  ngOnInit(): void {
    this.newsService.getAll().subscribe({
      next: (data) => {
        this.newsList = data;
        this.loading = false;
      },
      error: () => {
        this.error = 'Failed to load news';
        this.loading = false;
      }
    });
  }

  getImageUrl(imagePath?: string): string {
    return imagePath ? `http://localhost:5228/${imagePath}` : 'assets/no-image.png';
  }

  exportToExcel(): void {
    const ws = XLSX.utils.json_to_sheet(
      this.newsList.map(n => ({
        Title: n.title,
        Description: n.shortDescription,
        Created: new Date(n.createdDate || '').toLocaleDateString(),
        Status: n.status === 1 ? 'Active' : 'Inactive'
      }))
    );
    const wb = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, 'News');
    const excelBuffer = XLSX.write(wb, { bookType: 'xlsx', type: 'array' });
    const blob = new Blob([excelBuffer], { type: 'application/octet-stream' });
    saveAs(blob, 'NewsList.xlsx');
  }

  exportToPDF(): void {
    const doc = new jsPDF();
    const headers = [['Title', 'Description', 'Created Date', 'Status']];
    const data = this.newsList.map(n => [
      n.title,
      n.shortDescription || '',
      new Date(n.createdDate || '').toLocaleDateString(),
      n.status === 1 ? 'Active' : 'Inactive'
    ]);
    autoTable(doc, {
      head: headers,
      body: data,
      styles: { fontSize: 10 },
      headStyles: { fillColor: [41, 128, 185] }
    });
    doc.save('NewsList.pdf');
  }
}
