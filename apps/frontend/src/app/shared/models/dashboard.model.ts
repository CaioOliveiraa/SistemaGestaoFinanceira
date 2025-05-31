// Resumo geral de entradas, saidas e saldo;
export interface DashboardSummary {
    income: number;
    expense: number;
    balance: number;
}

// Resumo mensal: mee/ano + valores
export interface DashboardMonthly {
    month: string;
    income: number;
    expense: number;
    balance: number;
}

// Total por categorias do mes atual
export interface DashboardByCategory {
    category: string;
    type: 'income' | 'expense';
    total: number;
}

// Movimento diario nos ultimos 30 dias
export interface DashboardDaily {
    date: string;
    income: number;
    expense: number;
}
