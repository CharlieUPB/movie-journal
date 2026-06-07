import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthSessionStorage } from '../auth/auth-session.storage';

@Component({
  selector: 'app-navbar',
  imports: [RouterLink, RouterLinkActive],
  template: `
    <header class="app-navbar">
      <a class="brand" routerLink="/movie-reviews" aria-label="Movie Review Journal">
        <span class="brand-mark">MJ</span>
        <span>Movie Review Journal</span>
      </a>

      <nav class="nav-links" aria-label="Primary navigation">
        <a
          routerLink="/movie-reviews"
          routerLinkActive="active"
          [routerLinkActiveOptions]="{ exact: true }"
        >
          Published Reviews
        </a>
        @if (session.isLoggedIn()) {
          <a
            routerLink="/movie-reviews/my"
            routerLinkActive="active"
            [routerLinkActiveOptions]="{ exact: true }"
          >
            My Reviews
          </a>
          <a class="btn btn-primary" routerLink="/movie-review/create">Create Review</a>
          <button class="btn btn-ghost" type="button" (click)="logout()">Logout</button>
        } @else {
          <a class="btn btn-ghost" routerLink="/login">Login</a>
          <a class="btn btn-danger" routerLink="/register">Register</a>
        }
      </nav>
    </header>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class NavbarComponent {
  protected readonly session = inject(AuthSessionStorage);
  private readonly router = inject(Router);

  logout(): void {
    this.session.clearSession();
    void this.router.navigateByUrl('/movie-reviews');
  }
}
