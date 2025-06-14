import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { Transaction } from '../../../shared/models/transaction.model';
import { TransactionService } from '../../../core/services/transaction.service';

@Component({
    standalone: true,
    imports: [CommonModule, RouterModule],
    selector: 'app-transaction-list',
    templateUrl: './transaction-list.component.html',
    styleUrls: ['./transaction-list.component.scss'],
})
export class TransactionListComponent implements OnInit {
    transactions: Transaction[] = [];

    constructor(
        private transactionService: TransactionService,
        private router: Router
    ) {}

    ngOnInit(): void {
        this.loadTransactions();
    }

    private loadTransactions(): void {
        this.transactionService.getAll().subscribe({
            next: data => (this.transactions = data),
            error: err => console.log('Erro ao carregar transações', err),
        });
    }

    onEdit(id: string): void {
        this.router.navigateByUrl(`transactions/edit/${id}`);
    }

    onDelete(id: string): void {
        this.transactionService.delete(id).subscribe({
            next: () => this.loadTransactions(),
            error: err => console.log('Erro ao deletar transação', err),
        });
    }

    onCreate(): void {
        this.router.navigateByUrl('transactions/create');
    }

    transactionType(type: string): string {
        return type == '0' ? 'Receita' : 'Despesa';
    }
}
