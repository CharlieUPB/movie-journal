import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
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
        <a routerLink="/movie-reviews" routerLinkActive="active">Published Reviews</a>
        @if (session.isLoggedIn()) {
          <a routerLink="/movie-reviews" routerLinkActive="active">My Reviews</a>
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

  logout(): void {
    this.session.clearSession();
  }
}
