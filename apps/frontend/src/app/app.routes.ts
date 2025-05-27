import { Routes } from '@angular/router';
import { authGuard } from './core/auth.guard';
import { LoginComponent } from './features/auth/login/login.component';

export const routes: Routes = [
    { path: 'login', component: LoginComponent },
    {
        path: '',
        canMatch: [authGuard],
        children: [{ path: '', pathMatch: 'full', redirectTo: 'dashboard' }],
    },
];
