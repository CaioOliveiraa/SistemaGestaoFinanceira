import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { Observable } from 'rxjs';

export interface NotificationDto {
    to: string;
    subject: string;
    body: string;
}

@Injectable({ providedIn: 'root' })
export class NotificationService {
    constructor(private api: ApiService) {}

    send(dto: NotificationDto): Observable<void> {
        return this.api.post<void>('notifications/send', dto);
    }
}
