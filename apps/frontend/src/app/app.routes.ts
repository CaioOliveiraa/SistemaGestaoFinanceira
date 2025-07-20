import { Routes } from '@angular/router';
import { AuthLayoutComponent } from './layouts/auth-layout.component';
import { AppLayoutComponent } from './layouts/app-layout.component';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { ForgotPasswordComponent } from './features/auth/forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './features/auth/reset-password/reset-password.component';
import { OauthCallbackComponent } from './features/auth/oauth-callback/oauth-callback.component';
import { authGuard } from './core/auth.guard';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { CategoryListComponent } from './features/categories/category-list/category-list.component';
import { CategoryFormComponent } from './features/categories/category-form/category-form.component';
import { TransactionListComponent } from './features/transactions/transaction-list/transaction-list.component';
import { TransactionFormComponent } from './features/transactions/transaction-form/transaction-form.component';

export const routes: Routes = [
    {
        path: 'auth',
        component: AuthLayoutComponent,
        children: [
            { path: 'login', component: LoginComponent },
            { path: 'register', component: RegisterComponent },
            { path: 'forgot-password', component: ForgotPasswordComponent },
            {
                path: 'reset-password',
                component: ResetPasswordComponent,
            },
            { path: 'oauth-callback', component: OauthCallbackComponent },
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
            {
                path: 'categories',
                pathMatch: 'full',
                redirectTo: 'categories/list',
            },
            { path: 'categories/list', component: CategoryListComponent },
            { path: 'categories/create', component: CategoryFormComponent },
            { path: 'categories/edit/:id', component: CategoryFormComponent },
            {
                path: 'transactions',
                pathMatch: 'full',
                redirectTo: 'transactions/list',
            },
            { path: 'transactions/list', component: TransactionListComponent },
            {
                path: 'transactions/create',
                component: TransactionFormComponent,
            },
            {
                path: 'transactions/edit/:id',
                component: TransactionFormComponent,
            },
            { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
        ],
    },
    // Rotas “catch-all”
    { path: '**', redirectTo: 'auth/login' },
];
