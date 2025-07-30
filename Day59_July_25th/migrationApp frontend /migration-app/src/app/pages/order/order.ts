import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import jsPDF from 'jspdf';
import autoTable from 'jspdf-autotable';
import { CommonModule } from '@angular/common';
import { ProductService, ProductDto } from '../products/product.service'; // adjust path as needed

interface OrderDto {
  orderID: number;
  orderName: string;
  orderDate: string;
  paymentType: string;
  status: string;
  customerName: string;
  customerPhone: string;
  customerEmail: string;
  customerAddress: string;
}

interface OrderDetailDto {
  orderID: number;
  productID: number;
  price: number;
  quantity: number;
}

@Component({
  selector: 'app-order',
  templateUrl: './order.html',
  styleUrls: ['./order.css'],
  imports: [CommonModule]
})
export class OrderListComponent implements OnInit {
  orders: OrderDto[] = [];
  orderDetailsMap: { [orderId: number]: OrderDetailDto[] } = {};
  productMap: { [id: number]: string } = {}; // id → productName

  constructor(private http: HttpClient, private productService: ProductService) {}

  ngOnInit(): void {
    this.loadProducts();
    this.loadOrders();
  }

  loadProducts(): void {
    this.productService.getAll().subscribe({
      next: (products) => {
        this.productMap = {};
        for (const product of products) {
          this.productMap[product.productId] = product.productName;
        }
      },
      error: () => console.error('Failed to load products')
    });
  }

  loadOrders(): void {
    this.http.get<OrderDto[]>('http://localhost:5228/api/orders').subscribe({
      next: (orders) => {
        this.orders = orders;
        for (const order of orders) {
          this.loadOrderDetails(order.orderID);
        }
      },
      error: () => alert('Failed to load orders')
    });
  }

  loadOrderDetails(orderId: number): void {
    this.http.get<OrderDetailDto[]>('http://localhost:5228/api/orderdetails').subscribe({
      next: (details) => {
        this.orderDetailsMap[orderId] = details.filter(d => d.orderID === orderId);
      },
      error: () => console.error('Failed to load order details for orderID:', orderId)
    });
  }

  downloadPdf(): void {
    const doc = new jsPDF();
    doc.setFontSize(16);
    doc.text('All Orders Report', 14, 15);

    let y = 25;

    this.orders.forEach((order, index) => {
      doc.setFontSize(12);
      doc.text(`Order #${order.orderID} - ${order.orderName}`, 14, y);
      y += 6;
      doc.text(`Date: ${new Date(order.orderDate).toLocaleString()}`, 14, y);
      y += 6;
      doc.text(`Customer: ${order.customerName} | ${order.customerEmail}`, 14, y);
      y += 6;
      doc.text(`Phone: ${order.customerPhone} | Address: ${order.customerAddress}`, 14, y);
      y += 6;
      doc.text(`Status: ${order.status} | Payment: ${order.paymentType}`, 14, y);
      y += 6;

      const details = this.orderDetailsMap[order.orderID] || [];

      if (details.length > 0) {
        autoTable(doc, {
          head: [['Product', 'Price', 'Quantity', 'Total']],
          body: details.map(d => [
            this.productMap[d.productID] || `Product ${d.productID}`,
            `₹${d.price}`,
            d.quantity,
            `₹${d.price * d.quantity}`
          ]),
          startY: y,
          theme: 'striped',
          styles: { fontSize: 10 }
        });
        y = (doc as any).lastAutoTable.finalY + 10;
      } else {
        doc.text('No order details available.', 14, y);
        y += 10;
      }

      if (y > 270 && index < this.orders.length - 1) {
        doc.addPage();
        y = 20;
      }
    });

    doc.save('all-orders.pdf');
  }
}
