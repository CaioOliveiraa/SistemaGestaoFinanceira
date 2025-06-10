import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { Category } from '../../../shared/models/category.model';
import { CategoryService } from '../../../core/services/category.service';

@Component({
    standalone: true,
    imports: [CommonModule, RouterModule],
    selector: 'app-category-list',
    templateUrl: './category-list.component.html',
    styleUrls: ['./category-list.component.scss'],
})
export class CategoryListComponent implements OnInit {
    categories: Category[] = [];

    constructor(
        private categoryService: CategoryService,
        private router: Router
    ) {}

    ngOnInit(): void {
        this.loadCategories();
    }

    private loadCategories(): void {
        this.categoryService.getAll().subscribe({
            next: data => (this.categories = data),
            error: err => console.log('Erro ao carregar categorias', err),
        });
    }

    onEdit(id: string): void {
        this.router.navigateByUrl(`categories/${id}`);
    }

    onDelete(id: string): void {
        if (!confirm('Deseja realmente deletar essa categoria?')) return;
        this.categoryService.delete(id).subscribe({
            next: () => this.loadCategories(),
            error: err => console.log('Erro ao deletar categoria', err),
        });
    }

    onCreate(): void {
        this.router.navigateByUrl('categories/create');
    }
}
