import { ChangeDetectionStrategy, Component } from '@angular/core';
import { NavbarComponent } from './navbar.component';

@Component({
  selector: 'app-shell',
  imports: [NavbarComponent],
  template: `
    <app-navbar />
    <main class="mx-auto w-full max-w-6xl px-4 py-6 sm:px-6 lg:px-8 lg:py-8">
      <ng-content />
    </main>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AppShellComponent {}
