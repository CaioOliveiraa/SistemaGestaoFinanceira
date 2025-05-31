export interface Category {
    id: string;
    name: string;
    type: 'Income' | 'Expense';
    fixed: boolean;
    userId: string;
    createdAt: string;
    updatedAt: string;
}
