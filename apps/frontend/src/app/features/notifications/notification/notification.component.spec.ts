import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
    ReactiveFormsModule,
    FormGroup,
    FormBuilder,
    Validators,
} from '@angular/forms';
import {
    NotificationService,
    NotificationDto,
} from '../../../core/services/notification.service';

@Component({
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule],
    selector: 'app-notification',
    templateUrl: './notification.component.html',
    styleUrls: ['./notification.component.scss'],
})
export class NotificationComponent {
    form: FormGroup;
    sending = false;

    constructor(
        private fb: FormBuilder,
        private notificationService: NotificationService
    ) {
        this.form = this.fb.group({
            to: ['', [Validators.required, Validators.email]],
            subject: ['', Validators.required],
            body: ['', Validators.required],
        });
    }

    onSubmit(): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }
        this.sending = true;
        const dto: NotificationDto = this.form.value;
        this.notificationService.send(dto).subscribe({
            next: () => {
                alert('Notificação enviada!');
                this.sending = false;
                this.form.reset();
            },
            error: err => {
                console.error('Erro ao enviar notificação', err);
                alert('Falha ao enviar e-mail.');
                this.sending = false;
            },
        });
    }
}
