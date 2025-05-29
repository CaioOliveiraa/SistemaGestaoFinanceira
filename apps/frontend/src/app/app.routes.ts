import { Routes } from '@angular/router';
import { authGuard } from './core/auth.guard';
import { LoginComponent } from '../app/features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';

export const routes: Routes = [
    { path: 'login', component: LoginComponent },
    { path: 'register', component: RegisterComponent },
    {
        path: '',
        canMatch: [authGuard],
        children: [{ path: '', pathMatch: 'full', redirectTo: 'dashboard' }],
    },
];
