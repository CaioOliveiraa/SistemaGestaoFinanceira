export interface Category {
    id: string;
    name: string;
    type: 'Income' | 'Expense';
    fixed: boolean;
    userId: string;
    createdAt: string;
    updatedAt: string;
}

export interface CreateCategoryDto {
    name: string;
    type: 'Income' | 'Expense';
    fixed: boolean;
}

export interface UpdateCategoryDto {
    name: string;
    type: 'Income' | 'Expense';
    fixed: boolean;
}
