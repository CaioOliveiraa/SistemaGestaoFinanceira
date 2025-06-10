// src/app/core/auth.guard.ts
import { CanMatchFn, Route, UrlSegment } from '@angular/router';
import { AuthService } from './services/auth.service';
import { Router } from '@angular/router';
import { catchError, map, of } from 'rxjs';
import { inject } from '@angular/core';

export const authGuard: CanMatchFn = () => {
    const auth = inject(AuthService);
    const router = inject(Router);

    if (auth.isLoggedIn()) {
        // já temos state em memória
        return true;
    }

    // tenta re-hidratar com /me
    return auth.me().pipe(
        map(user => (user ? true : router.createUrlTree(['/auth/login']))),
        catchError(() => of(router.createUrlTree(['/auth/login'])))
    );
};
