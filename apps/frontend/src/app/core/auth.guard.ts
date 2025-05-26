// auth.guard.ts
import { CanMatchFn, Route, UrlSegment } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from './services/auth.service';

/**
 * Bloqueia rotas para usuários não autenticados.
 * Usa `inject()` para pegar o AuthService em contexto standalone.
 */
export const authGuard: CanMatchFn = (route: Route, segments: UrlSegment[]) => {
    const auth = inject(AuthService);
    return auth.isLoggedIn();
};
