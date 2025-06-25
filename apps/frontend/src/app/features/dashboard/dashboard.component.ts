import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgChartsModule } from 'ng2-charts';
import { ChartConfiguration, ChartOptions } from 'chart.js';
import { DashboardService } from '../../core/services/dashboard.service';
import {
    DashboardSummary,
    DashboardMonthly,
    DashboardByCategory,
    DashboardDaily,
} from '../../shared/models/dashboard.model';
import { environment } from '../../../environments/environment';

@Component({
    standalone: true,
    imports: [CommonModule, NgChartsModule],
    selector: 'app-dashboard',
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.scss'],
})
export class DashboardComponent {
    summary!: DashboardSummary;
    monthlyData: DashboardMonthly[] = [];
    byCategoryData: DashboardByCategory[] = [];
    dailyData: DashboardDaily[] = [];

    // Chart
    public barChartOptions: ChartOptions = {
        responsive: true,
    };
    public barChartLabels: string[] = [];
    public barChartData: ChartConfiguration<'bar'>['data'] = {
        labels: [],
        datasets: [
            { label: 'Receita', data: [] },
            { label: 'Despesa', data: [] },
        ],
    };

    public pieChartOptions: ChartOptions<'pie'> = {
        responsive: true,
    };
    public pieChartLabels: string[] = [];
    public pieChartData: number[] = [];

    public lineChartOptions: ChartOptions<'line'> = {
        responsive: true,
    };
    public lineChartLabels: string[] = [];
    public lineChartData: ChartConfiguration<'line'>['data'] = {
        labels: [],
        datasets: [
            { label: 'Receita', data: [] },
            { label: 'Despesa', data: [] },
        ],
    };

    constructor(private dashboardService: DashboardService) {}

    ngOnInit(): void {
        this.loadSummary();
        this.loadMonthly();
        this.loadByCategory();
        this.loadDaily();
    }

    // Metodos

    exportExcel(): void {
        window.open(`${environment.apiUrl}/export/excel`, '_blank');
    }

    exportPdf(): void {
        window.open(`${environment.apiUrl}/export/pdf`, '_blank');
    }

    private loadSummary() {
        this.dashboardService.getSummary().subscribe({
            next: data => (this.summary = data),
            error: err => console.log('Erro ao carregar summary', err),
        });
    }

    private loadMonthly() {
        this.dashboardService.getMonthly().subscribe({
            next: data => {
                this.monthlyData = data;
                this.barChartLabels = data.map(item => item.month);
                this.barChartData = {
                    labels: this.barChartLabels,
                    datasets: [
                        {
                            label: 'Receita',
                            data: data.map(item => item.income),
                        },
                        {
                            label: 'Despesa',
                            data: data.map(item => item.expense),
                        },
                    ],
                };
            },
            error: err => console.log('Erro ao carregar Mensal', err),
        });
    }

    private loadByCategory() {
        this.dashboardService.getByCategory().subscribe({
            next: data => {
                this.byCategoryData = data;
                const labels = data.map(item => item.category);
                const values = data.map(item => item.total);
                this.pieChartLabels = labels;
                this.pieChartData = values;
            },
            error: err => console.log('Erro ao carregar por Categoria', err),
        });
    }

    private loadDaily() {
        this.dashboardService.getDaily().subscribe({
            next: data => {
                this.dailyData = data;
                this.lineChartLabels = data.map(item => item.date);
                this.lineChartData = {
                    labels: this.lineChartLabels,
                    datasets: [
                        {
                            label: 'Receita',
                            data: data.map(item => item.income),
                            tension: 0.4,
                        },
                        {
                            label: 'Despesa',
                            data: data.map(item => item.expense),
                            tension: 0.4,
                        },
                    ],
                };
            },
            error: err => console.log('Erro ao carregar Diario', err),
        });
    }
}
