import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { Observable } from 'rxjs';
import {
    DashboardSummary,
    DashboardMonthly,
    DashboardByCategory,
    DashboardDaily,
} from '../../shared/models/dashboard.model';

@Injectable({ providedIn: 'root' })
export class DashboardService {
    constructor(private api: ApiService) {}

    getSummary(): Observable<DashboardSummary> {
        return this.api.get<DashboardSummary>('dashboard/summary');
    }

    getMonthly(): Observable<DashboardMonthly[]> {
        return this.api.get<DashboardMonthly[]>('dashboard/monthly');
    }

    getByCategory(): Observable<DashboardByCategory[]> {
        return this.api.get<DashboardByCategory[]>('dashboard/by-category');
    }

    getDaily(): Observable<DashboardDaily[]> {
        return this.api.get<DashboardDaily[]>('dashboard/daily');
    }
}
