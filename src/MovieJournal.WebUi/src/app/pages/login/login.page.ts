import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthApi } from '../../data-access/users/auth.api';

@Component({
  selector: 'app-login-page',
  imports: [RouterLink],
  template: `
    <section class="page-panel">
      <p class="eyebrow">Welcome back</p>
      <h1>Login</h1>
      <p class="page-copy">Connect this page to a small login form when the first auth flow is wired.</p>

      <div class="placeholder-card">
        <span>API ready</span>
        <strong>{{ apiStatus() }}</strong>
      </div>

      <a class="text-link" routerLink="/register">Create an account</a>
    </section>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LoginPage {
  private readonly authApi = inject(AuthApi);
  protected readonly apiStatus = signal(this.authApi ? 'AuthApi injected' : 'Missing AuthApi');
}
