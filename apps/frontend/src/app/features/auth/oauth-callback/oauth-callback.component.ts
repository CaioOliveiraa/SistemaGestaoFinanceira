import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
    selector: 'app-oauth-callback',
    standalone: true,
    imports: [CommonModule, RouterModule],
    templateUrl: './oauth-callback.component.html',
    styleUrls: ['./oauth-callback.component.scss'],
})
export class OauthCallbackComponent implements OnInit {
    error = false;

    constructor(private auth: AuthService, private router: Router) {}

    ngOnInit(): void {
        const hasError = window.location.search.includes('error');
        if (hasError) {
            this.error = true;
            return;
        }

        this.auth.me().subscribe({
            next: () => this.router.navigateByUrl('/dashboard'),
            error: () => {
                this.error = true;
            },
        });
    }
}
