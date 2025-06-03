import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';

@Component({
    standalone: true,
    imports: [CommonModule, RouterOutlet],
    selector: 'app-auth-layout',
    template: `
        <div class="auth-wrapper">
            <router-outlet></router-outlet>
        </div>
    `,
})
export class AuthLayoutComponent {}
