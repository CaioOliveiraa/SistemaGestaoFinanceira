import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { LoginDto } from '../../shared/models/login.dto';
import { CreateUserDto } from '../../shared/models/create-user.dto';
import { UserResponseDto } from '../../shared/models/user-response.dto';
import { tap, Observable, BehaviorSubject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthService {
    private userSubject = new BehaviorSubject<UserResponseDto | null>(null);
    user$ = this.userSubject.asObservable();

    constructor(private api: ApiService) {}

    register(dto: CreateUserDto): Observable<UserResponseDto> {
        return this.api.post<UserResponseDto>('users', dto);
    }

    login(dto: LoginDto): Observable<UserResponseDto> {
        return this.api
            .post<UserResponseDto>('auth/login', dto)
            .pipe(tap(user => this.userSubject.next(user)));
    }

    logout(): Observable<void> {
        return this.api
            .post<void>('auth/logout', {})
            .pipe(tap(() => this.userSubject.next(null)));
    }

    me(): Observable<UserResponseDto> {
        return this.api
            .get<UserResponseDto>('auth/me')
            .pipe(tap(user => this.userSubject.next(user)));
    }

    isLoggedIn(): boolean {
        return this.userSubject.value !== null;
    }
}
