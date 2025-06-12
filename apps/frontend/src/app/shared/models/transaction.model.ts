export interface Transaction {
    id: string;
    type: 'Income' | 'Expense';
    amount: number;
    description: string;
    date: string;
    recurring: boolean;
    categoryId: string;
    categoryName: string;
    userId: string;
    createdAt: string;
    updatedAt: string;
}

export interface CreateTransactionDto {
    type: number;
    amount: number;
    description: string;
    date: string;
    recurring: boolean;
    categoryId: string;
}

export interface UpdateTransactionDto {
    type: number;
    amount: number;
    description: string;
    date: string;
    recurring: boolean;
    categoryId: string;
}
