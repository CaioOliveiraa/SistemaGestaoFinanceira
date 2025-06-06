export interface Transaction {
    id: string;
    type: 'Income' | 'Expense';
    amount: number;
    description: string;
    date: string;
    recurring: boolean;
    categoryId: string;
    userId: string;
    createdAt: string;
    updatedAt: string;
}

export interface CreateTransactionDto {
    type: 'Income' | 'Expense';
    amount: number;
    description: string;
    date: string;
    recurring: boolean;
    categoryId: string;
}

export interface UpdateTransactionDto {
    type: 'Income' | 'Expense';
    amount: number;
    description: string;
    date: string;
    recurring: boolean;
    categoryId: string;
}
