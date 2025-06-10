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
import { CategoryService } from '../../../core/services/category.service';

@Component({
    standalone: true,
    imports: [CommonModule, RouterModule, ReactiveFormsModule],
    selector: 'app-category-form',
    templateUrl: './category-form.component.html',
    styleUrls: ['./category-form.component.scss'],
})
export class CategoryFormComponent implements OnInit {
    form: FormGroup;
    isEditMode = false;
    categoryId: string | null = null;

    constructor(
        private fb: FormBuilder,
        private categoryService: CategoryService,
        private router: Router,
        private route: ActivatedRoute
    ) {
        this.form = this.fb.group({
            name: ['', [Validators.required, Validators.minLength(3)]],
            type: [0, Validators.required],
            fixed: [false],
        });
    }

    ngOnInit(): void {
        this.categoryId = this.route.snapshot.paramMap.get('id');
        if (this.categoryId) {
            this.isEditMode = true;
            this.loadCategory(this.categoryId);
        }
    }

    private loadCategory(id: string): void {
        this.categoryService.getById(id).subscribe({
            next: (cat: Category) => {
                this.form.patchValue({
                    name: cat.name,
                    type: cat.type,
                    fixed: cat.fixed,
                });
            },
            error: () => {
                this.router.navigateByUrl('/categories');
            },
        });
    }

    onSubmit(): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        const data = this.form.value as {
            name: string;
            type: 'Income' | 'Expense';
            fixed: boolean;
        };

        if (this.isEditMode && this.categoryId) {
            this.categoryService.update(this.categoryId, data).subscribe({
                next: () => this.router.navigateByUrl('/categories/list'),
                error: err => console.log('Erro ao atualizar categoria', err),
            });
        } else {
            this.categoryService.create(data).subscribe({
                next: () => this.router.navigateByUrl('/categories/list'),
                error: err => console.log('Erro ao criar categoria', err),
            });
        }
    }

    onCancel(): void {
        this.router.navigateByUrl('/categories/list');
    }
}
