// src/app/app.config.ts
import {
    ApplicationConfig,
    APP_INITIALIZER,
    importProvidersFrom,
} from '@angular/core';
import { provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import {
    provideClientHydration,
    withEventReplay,
} from '@angular/platform-browser';
import {
    provideHttpClient,
    withFetch,
    withInterceptors,
} from '@angular/common/http';

import { routes } from './app.routes';
import { apiInterceptor } from './core/api.interceptor';
import { AuthService } from './core/services/auth.service';

// factory que chama /me() e só resolve depois
export function initAuthFactory(auth: AuthService) {
    return () =>
        auth
            .me()
            .toPromise()
            .catch(() => null);
}

export const appConfig: ApplicationConfig = {
    providers: [
        provideZoneChangeDetection({ eventCoalescing: true }),
        provideRouter(routes),
        provideClientHydration(withEventReplay()),
        // registra o interceptor que adiciona withCredentials
        provideHttpClient(withInterceptors([apiInterceptor]), withFetch()),
        // chama auth.me() antes do app bootar
        {
            provide: APP_INITIALIZER,
            useFactory: initAuthFactory,
            deps: [AuthService],
            multi: true,
        },
    ],
};
