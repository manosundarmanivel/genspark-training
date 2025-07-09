import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminService } from '../services/admin.service';

@Component({
  selector: 'app-transaction-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './transaction-list.component.html',
})
export class TransactionListComponent {
  transactions$: ReturnType<AdminService['getAllTransactions']>;

  constructor(private adminService: AdminService) {
    this.transactions$ = this.adminService.getAllTransactions();
  }
}
