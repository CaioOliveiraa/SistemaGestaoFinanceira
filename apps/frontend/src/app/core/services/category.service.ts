import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { Observable } from 'rxjs';
import {
    Category,
    CreateCategoryDto,
    UpdateCategoryDto,
} from '../../shared/models/category.model';

@Injectable({ providedIn: 'root' })
export class CategoryService {
    constructor(private api: ApiService) {}

    // GET /categories
    getAll(): Observable<Category[]> {
        return this.api.get<Category[]>('categories');
    }

    // GET /categories/:id
    getById(id: string): Observable<Category> {
        return this.api.get<Category>(`categories/${id}`);
    }

    // POST /categories
    create(dto: CreateCategoryDto): Observable<Category> {
        return this.api.post<Category>('categories', dto);
    }

    // PUT /categories/:id
    update(id: string, dto: UpdateCategoryDto): Observable<Category> {
        return this.api.put<Category>(`categories/${id}`, dto);
    }

    // DELETE /categories/:id
    delete(id: string): Observable<void> {
        return this.api.delete<void>(`categories/${id}`);
    }
}
