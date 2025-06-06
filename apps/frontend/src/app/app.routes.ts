import { Routes } from '@angular/router';
import { AuthLayoutComponent } from './layouts/auth-layout.component';
import { AppLayoutComponent } from './layouts/app-layout.component';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { authGuard } from './core/auth.guard';
import { DashboardComponent } from './features/dashboard/dashboard.component';

export const routes: Routes = [
    {
        path: 'auth',
        component: AuthLayoutComponent,
        children: [
            { path: 'login', component: LoginComponent },
            { path: 'register', component: RegisterComponent },
            { path: '', pathMatch: 'full', redirectTo: 'login' },
        ],
    },
    {
        path: '',
        component: AppLayoutComponent,
        canMatch: [authGuard],
        children: [
            {
                path: 'dashboard',
                pathMatch: 'full',
                component: DashboardComponent,
            },
            // ...
            { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
        ],
    },
    // Rotas “catch-all”
    { path: '**', redirectTo: 'auth/login' },
];
