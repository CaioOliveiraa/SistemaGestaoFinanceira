import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { LoginDto } from '../../shared/models/login.dto';
import { CreateUserDto } from '../../shared/models/create-user.dto';
import { UserResponseDto } from '../../shared/models/user-response.dto';
import { tap, Observable } from 'rxjs';

/**
 * Encapsula endpoints de autenticação:
 * - /users      → register
 * - /auth/login → login
 * - /auth/logout→ logout
 * - /me         → obter dados do usuário logado
 */

@Injectable({ providedIn: 'root' })
export class AuthService {
    private currentUser: UserResponseDto | null = null;

    constructor(private api: ApiService) {}

    register(dto: CreateUserDto): Observable<UserResponseDto> {
        return this.api.post<UserResponseDto>('users', dto);
    }

    login(dto: LoginDto): Observable<UserResponseDto> {
        return this.api
            .post<UserResponseDto>('auth/login', dto)
            .pipe(tap(user => (this.currentUser = user)));
    }

    logout(): Observable<void> {
        return this.api
            .delete<void>('auth/logout')
            .pipe(tap(() => (this.currentUser = null)));
    }

    me(): Observable<UserResponseDto> {
        return this.api.get<UserResponseDto>('auth/me');
    }

    isLoggedIn(): boolean {
        return this.currentUser !== null;
    }
}
