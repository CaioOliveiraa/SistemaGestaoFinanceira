import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
/**
 * Centraliza todas as chamadas HTTP para a API.
 * Usa `environment.apiUrl` como base (ex: "https://localhost:5062/api").
 */

@Injectable({ providedIn: 'root' })
export class ApiService {
    private readonly baseUrl = environment.apiUrl;

    constructor(private http: HttpClient) {}

    get<T>(path: string) {
        return this.http.get<T>(`${this.baseUrl}/${path}`);
    }

    post<T>(path: string, body: any) {
        return this.http.post<T>(`${this.baseUrl}/${path}`, body);
    }

    put<T>(path: string, body: any) {
        return this.http.put<T>(`${this.baseUrl}/${path}`, body);
    }

    delete<T>(path: string) {
        return this.http.delete<T>(`${this.baseUrl}/${path}`);
    }
}
