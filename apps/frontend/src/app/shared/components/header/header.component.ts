import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../core/services/auth.service';
import { Router } from '@angular/router';

@Component({
    standalone: true,
    imports: [CommonModule],
    selector: 'app-header',
    templateUrl: './header.component.html',
    styleUrls: ['./header.component.scss'],
})
export class HeaderComponent {
    constructor(private auth: AuthService, private router: Router) {}

    logout() {
        this.auth.logout().subscribe({
            next: () => this.router.navigateByUrl('/login'),
            error: () => this.router.navigateByUrl('/login'),
        });
    }
}
