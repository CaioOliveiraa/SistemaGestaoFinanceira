// app-layout.component.ts
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from '../shared/components/header/header.component';

@Component({
    standalone: true,
    imports: [CommonModule, RouterOutlet, HeaderComponent],
    selector: 'app-app-layout',
    template: `
        <app-header></app-header>
        <main class="app-content">
            <router-outlet></router-outlet>
        </main>
    `,
})
export class AppLayoutComponent {}
