import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
    FormBuilder,
    FormGroup,
    Validators,
    ReactiveFormsModule,
} from '@angular/forms';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { error } from 'console';

@Component({
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, RouterModule],
    selector: 'app-forgot-password',
    templateUrl: './forgot-password.component.html',
    styleUrls: ['./forgot-password.component.scss'],
})
export class ForgotPasswordComponent {
    form: FormGroup;
    submitted = false;
    errorMsg: string | null = null;
    successMsg: string | null = null;

    constructor(private fb: FormBuilder, private auth: AuthService) {
        this.form = this.fb.group({
            email: ['', [Validators.required, Validators.email]],
        });
    }

    onSubmit(): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }
        this.submitted = true;
        this.errorMsg = null;

        this.auth.forgotPassword(this.form.value.email).subscribe({
            next: () => {
                this.successMsg =
                    'Se enviamos um e-mail, verifique sua caixa de entrada.';
                this.submitted = false;
            },
            error: (err: any) => {
                this.errorMsg =
                    err.error?.error || 'Erro ao solicitar recuperação.';
                this.submitted = false;
            },
        });
    }
}
