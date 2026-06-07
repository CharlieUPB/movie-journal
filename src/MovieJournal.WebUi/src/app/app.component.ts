import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AppShellComponent } from './core/layout/app-shell.component';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, AppShellComponent],
  template: `
    <app-shell>
      <router-outlet />
    </app-shell>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AppComponent {}
