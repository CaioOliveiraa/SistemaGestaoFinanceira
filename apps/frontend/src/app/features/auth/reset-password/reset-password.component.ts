// apps/frontend/src/app/features/auth/reset-password/reset-password.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
    ReactiveFormsModule,
    FormBuilder,
    FormGroup,
    Validators,
} from '@angular/forms';
import { RouterModule, ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, RouterModule],
    selector: 'app-reset-password',
    templateUrl: './reset-password.component.html',
    styleUrls: ['./reset-password.component.scss'],
})
export class ResetPasswordComponent implements OnInit {
    form: FormGroup;
    token = '';
    submitting = false;
    errorMsg: string | null = null;
    successMsg: string | null = null;

    constructor(
        private fb: FormBuilder,
        private route: ActivatedRoute,
        private auth: AuthService,
        private router: Router
    ) {
        this.form = this.fb.group(
            {
                newPassword: [
                    '',
                    [Validators.required, Validators.minLength(6)],
                ],
                confirmPassword: ['', [Validators.required]],
            },
            { validators: this.passwordsMatch }
        );
    }

    ngOnInit(): void {
        this.token = this.route.snapshot.queryParamMap.get('token') || '';
        if (!this.token) {
            this.errorMsg = 'Token de recuperação ausente ou inválido.';
        }
    }

    private passwordsMatch(group: FormGroup) {
        const pwd = group.get('newPassword')?.value;
        const confirm = group.get('confirmPassword')?.value;
        return pwd === confirm ? null : { mismatch: true };
    }

    onSubmit(): void {
        // bloqueia se inválido ou sem token
        if (this.form.invalid || !this.token) {
            this.form.markAllAsTouched();
            return;
        }

        this.submitting = true;
        this.errorMsg = this.successMsg = null;

        const newPwd = this.form.get('newPassword')!.value;
        this.auth.resetPassword(this.token, newPwd).subscribe({
            next: () => {
                this.successMsg =
                    'Senha alterada com sucesso! Redirecionando ao login…';
                setTimeout(() => this.router.navigate(['/auth/login']), 2000);
            },
            error: err => {
                this.errorMsg = err.error?.error || 'Erro ao alterar senha.';
                this.submitting = false;
            },
        });
    }
}
