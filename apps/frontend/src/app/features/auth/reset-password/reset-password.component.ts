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
    token!: string;
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
            { Validators: this.passwordMatch }
        );
    }

    ngOnInit(): void {
        this.token = this.route.snapshot.queryParamMap.get('token') || '';
        if (!this.token) {
            this.errorMsg = 'Token de recuperação inválido.';
        }
    }

    private passwordMatch(group: FormGroup) {
        const a = group.get('newPassword')?.value;
        const b = group.get('confirmPassword')?.value;
        return a === b ? null : { mismatch: true };
    }

    onSubmit(): void {
        if (this.form.invalid || !this.token) {
            this.form.markAllAsTouched();
            return;
        }
        this.submitting = true;
        this.errorMsg = this.successMsg = null;

        this.auth
            .resetPassword(this.token, this.form.get('newPassword')!.value)
            .subscribe({
                next: () => {
                    this.successMsg = 'Senha alterada com sucesso!';
                    setTimeout(
                        () => this.router.navigate(['/auth/login']),
                        2000
                    );
                },
                error: (err: any) => {
                    this.errorMsg =
                        err.error?.error || 'Erro ao alterar senha.';
                    this.submitting = false;
                },
            });
    }
}
