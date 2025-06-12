import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
    FormBuilder,
    ReactiveFormsModule,
    FormGroup,
    Validators,
} from '@angular/forms';
import { RouterModule, Router, ActivatedRoute } from '@angular/router';
import { Category } from '../../../shared/models/category.model';
import { Transaction } from '../../../shared/models/transaction.model';
import { TransactionService } from '../../../core/services/transaction.service';
import { CategoryService } from '../../../core/services/category.service';

@Component({
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, RouterModule],
    selector: 'app-transaction-form',
    templateUrl: './transaction-form.component.html',
    styleUrls: ['./transaction-form.component.scss'],
})
export class TransactionFormComponent implements OnInit {
    form: FormGroup = new FormGroup({});
    isEditMode = false;
    transactionId: string | null = null;
    categories: Category[] = [];

    constructor(
        private fb: FormBuilder,
        private transactionService: TransactionService,
        private categoryService: CategoryService,
        private router: Router,
        private route: ActivatedRoute
    ) {
        this.form = this.fb.group({
            description: ['', [Validators.required]],
            amount: ['', [Validators.required, Validators.min(0.01)]],
            date: ['', [Validators.required]],
            type: [1, [Validators.required]],
            recurring: [false],
            categoryId: ['', [Validators.required]],
        });
    }

    ngOnInit(): void {
        // Carrega as categorias
        this.loadCategories();

        // Verifica modo edição
        this.transactionId = this.route.snapshot.paramMap.get('id');
        if (this.transactionId) {
            this.isEditMode = true;
            this.loadTransaction(this.transactionId);
        }
    }

    private loadCategories(): void {
        this.categoryService.getAll().subscribe({
            next: data => (this.categories = data),
            error: err => console.log('Erro ao carregar categorias', err),
        });
    }

    private loadTransaction(id: string): void {
        this.transactionService.getById(id).subscribe({
            next: tx => {
                this.form.patchValue({
                    description: tx.description,
                    amount: tx.amount,
                    date: tx.date.split('T')[0],
                    type: tx.type,
                    recurring: tx.recurring,
                    categoryId: tx.categoryId,
                });
            },
            error: err => console.log('Erro ao carregar transação', err),
        });
    }

    onSubmit(): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        const data = this.form.value as {
            description: string;
            amount: number;
            date: string;
            type: number;
            recurring: boolean;
            categoryId: string;
        };

        if (this.isEditMode && this.transactionId) {
            this.transactionService.update(this.transactionId, data).subscribe({
                next: () => this.router.navigateByUrl('/transactions/list'),
                error: err => console.log('Erro ao atualizar transação', err),
            });
        } else {
            console.log(data);
            this.transactionService.create(data).subscribe({
                next: () => this.router.navigateByUrl('/transactions/list'),
                error: err => console.log('Erro ao criar transação', err),
            });
        }
    }

    onCancel(): void {
        this.router.navigateByUrl('/transactions/list');
    }
}
