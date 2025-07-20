import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
    FormBuilder,
    ReactiveFormsModule,
    FormGroup,
    Validators,
} from '@angular/forms';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { environment } from '../../../../environments/environment';

@Component({
    selector: 'app-register',
    imports: [CommonModule, ReactiveFormsModule, RouterModule],
    templateUrl: './register.component.html',
    styleUrl: './register.component.scss',
})
export class RegisterComponent {
    form: FormGroup;
    oauthGoogleUrl = `${environment.apiUrl}/auth/oauth/google`;

    constructor(
        private fb: FormBuilder,
        private router: Router,
        private auth: AuthService
    ) {
        this.form = this.fb.group({
            name: ['', [Validators.required, Validators.minLength(3)]],
            email: ['', [Validators.required, Validators.email]],
            password: ['', [Validators.required, Validators.minLength(6)]],
        });
    }

    onSubmit() {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        const { name, email, password } = this.form.value;

        this.auth.register({ name, email, password }).subscribe({
            next: () => {
                this.router.navigateByUrl('/login');
            },
            error: err => {
                alert(err.error.message || 'Erro ao fazer cadastro');
            },
        });
    }
}
