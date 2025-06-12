import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { Observable } from 'rxjs';
import {
    Transaction,
    CreateTransactionDto,
    UpdateTransactionDto,
} from '../../shared/models/transaction.model';

@Injectable({ providedIn: 'root' })
export class TransactionService {
    constructor(private api: ApiService) {}

    getAll(): Observable<Transaction[]> {
        return this.api.get<Transaction[]>('transactions');
    }

    getById(id: string): Observable<Transaction> {
        return this.api.get<Transaction>(`transactions/${id}`);
    }

    create(dto: CreateTransactionDto): Observable<Transaction> {
        return this.api.post<Transaction>('transactions', dto);
    }

    update(id: string, dto: UpdateTransactionDto): Observable<Transaction> {
        return this.api.put<Transaction>(`transactions/${id}`, dto);
    }

    delete(id: string): Observable<void> {
        return this.api.delete<void>(`transactions/${id}`);
    }
}
