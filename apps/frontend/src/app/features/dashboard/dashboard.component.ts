import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgChartsModule } from 'ng2-charts';
import {
    Chart,
    ArcElement,
    Legend,
    Tooltip,
    ChartConfiguration,
    ChartOptions,
} from 'chart.js';
import { DashboardService } from '../../core/services/dashboard.service';
import {
    DashboardSummary,
    DashboardMonthly,
    DashboardByCategory,
    DashboardDaily,
} from '../../shared/models/dashboard.model';
import { environment } from '../../../environments/environment';

// registra elementos necessários para legendas e tooltips
Chart.register(ArcElement, Legend, Tooltip);

@Component({
    standalone: true,
    imports: [CommonModule, NgChartsModule],
    selector: 'app-dashboard',
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.scss'],
})
export class DashboardComponent implements OnInit {
    summary!: DashboardSummary;
    monthlyData: DashboardMonthly[] = [];
    byCategoryData: DashboardByCategory[] = [];
    dailyData: DashboardDaily[] = [];

    /** Configuração visual comum a todos os gráficos */
    private commonLegend = {
        position: 'bottom' as const,
        align: 'center' as const,
        labels: {
            usePointStyle: true,
            pointStyle: 'circle' as const,
            boxWidth: 10,
            boxHeight: 10,
            padding: 20,
            font: { family: 'Arial, sans-serif', size: 14, weight: 500 },
            color: '#4a4a4a',
        },
    };

    private commonTooltip = {
        enabled: true,
        titleFont: { family: 'Arial, sans-serif', size: 14, weight: 500 },
        bodyFont: { family: 'Arial, sans-serif' },
    };

    private commonLayout = { padding: { top: 30, bottom: 20 } };

    /** Remove bordas brancas dos pontos (legendas e tooltips) */
    private commonElements = { point: { borderWidth: 0 } };

    // -- opções de gráfico de barras --
    public barChartOptions: ChartOptions<'bar'> = {
        responsive: true,
        layout: this.commonLayout,
        elements: this.commonElements,
        plugins: {
            legend: this.commonLegend,
            tooltip: this.commonTooltip,
        },
    };

    // -- opções de gráfico de pizza --
    public pieChartOptions: ChartOptions<'pie'> = {
        responsive: true,
        layout: this.commonLayout,
        elements: this.commonElements,
        plugins: {
            legend: this.commonLegend,
            tooltip: this.commonTooltip,
        },
    };

    // -- opções de gráfico de linha --
    public lineChartOptions: ChartOptions<'line'> = {
        responsive: true,
        layout: this.commonLayout,
        elements: this.commonElements,
        plugins: {
            legend: this.commonLegend,
            tooltip: this.commonTooltip,
        },
        scales: {
            x: { grid: { display: false } },
            y: { grid: { display: false } },
        },
    };

    public barChartLabels: string[] = [];
    public barChartData: ChartConfiguration<'bar'>['data'] = {
        labels: [],
        datasets: [
            { label: 'Receita', data: [], borderRadius: 4 },
            { label: 'Despesa', data: [], borderRadius: 4 },
        ],
    };

    public pieChartLabels: string[] = [];
    public pieChartData: number[] = [];
    public pieChartDatasetLabel = 'Gastos por Categoria';

    public lineChartType: 'line' = 'line';
    public lineChartLabels: string[] = [];
    public lineChartData: ChartConfiguration<'line'>['data'] = {
        labels: [],
        datasets: [
            {
                label: 'Receita',
                data: [],
                tension: 0.4,
                pointRadius: 4,
                pointHoverRadius: 6,
            },
            {
                label: 'Despesa',
                data: [],
                tension: 0.4,
                pointRadius: 4,
                pointHoverRadius: 6,
            },
        ],
    };

    constructor(private dashboardService: DashboardService) {}

    ngOnInit(): void {
        this.loadSummary();
        this.loadMonthly();
        this.loadByCategory();
        this.loadDaily();
    }

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
                            borderRadius: 4,
                        },
                        {
                            label: 'Despesa',
                            data: data.map(item => item.expense),
                            borderRadius: 4,
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
                this.pieChartLabels = data.map(item => item.category);
                this.pieChartData = data.map(item => item.amount);
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
                            pointRadius: 4,
                            pointHoverRadius: 6,
                        },
                        {
                            label: 'Despesa',
                            data: data.map(item => item.expense),
                            tension: 0.4,
                            pointRadius: 4,
                            pointHoverRadius: 6,
                        },
                    ],
                };
            },
            error: err => console.log('Erro ao carregar Diário', err),
        });
    }
}
