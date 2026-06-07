import { ChangeDetectionStrategy, Component } from '@angular/core';
import { NavbarComponent } from './navbar.component';

@Component({
  selector: 'app-shell',
  imports: [NavbarComponent],
  template: `
    <app-navbar />
    <main class="app-main">
      <ng-content />
    </main>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AppShellComponent {}
