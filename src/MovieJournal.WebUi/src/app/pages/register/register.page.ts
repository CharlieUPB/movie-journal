import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthApi } from '../../data-access/users/auth.api';

@Component({
  selector: 'app-register-page',
  imports: [RouterLink],
  template: `
    <section class="page-panel">
      <p class="eyebrow">Join the journal</p>
      <h1>Register</h1>
      <p class="page-copy">This placeholder is ready for display name, email, and password fields.</p>

      <div class="placeholder-card">
        <span>API ready</span>
        <strong>{{ apiStatus() }}</strong>
      </div>

      <a class="text-link" routerLink="/login">Already have an account?</a>
    </section>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RegisterPage {
  private readonly authApi = inject(AuthApi);
  protected readonly apiStatus = signal(this.authApi ? 'AuthApi injected' : 'Missing AuthApi');
}
