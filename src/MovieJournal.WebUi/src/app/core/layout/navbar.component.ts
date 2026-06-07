import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthSessionStorage } from '../auth/auth-session.storage';
import { ThemeService } from '../theme/theme.service';

@Component({
  selector: 'app-navbar',
  imports: [RouterLink, RouterLinkActive],
  template: `
    <header class="sticky top-0 z-20 border-b border-violet-100/80 bg-white/90 shadow-sm backdrop-blur dark:border-violet-900/60 dark:bg-[#0d0a14]/90">
      <div class="mx-auto flex min-h-[4.5rem] w-full max-w-6xl flex-col gap-3 px-4 py-3 sm:px-6 lg:flex-row lg:items-center lg:justify-between lg:px-8">
        <a class="flex items-center gap-3 text-xl font-extrabold text-slate-950 no-underline dark:text-violet-50" routerLink="/movie-reviews" aria-label="Movie Review Journal">
          <span class="grid size-10 place-items-center rounded-full bg-gradient-to-br from-violet-600 to-indigo-700 text-xs font-black text-white shadow-lg shadow-violet-600/25">MJ</span>
          <span>Movie Review Journal</span>
        </a>

        <nav class="flex flex-col gap-2 lg:flex-row lg:items-center" aria-label="Primary navigation">
          <a
            class="rounded-md border border-transparent px-3 py-2 text-sm font-bold text-slate-600 no-underline transition hover:text-violet-700 dark:text-violet-200 dark:hover:text-violet-100"
            routerLink="/movie-reviews"
            routerLinkActive="border-violet-600 text-violet-700 dark:border-violet-400 dark:text-violet-100"
            [routerLinkActiveOptions]="{ exact: true }"
          >
            Published Reviews
          </a>
          @if (session.isLoggedIn()) {
            <a
              class="rounded-md border border-transparent px-3 py-2 text-sm font-bold text-slate-600 no-underline transition hover:text-violet-700 dark:text-violet-200 dark:hover:text-violet-100"
              routerLink="/movie-reviews/my"
              routerLinkActive="border-violet-600 text-violet-700 dark:border-violet-400 dark:text-violet-100"
              [routerLinkActiveOptions]="{ exact: true }"
            >
              My Reviews
            </a>
            <a class="inline-flex min-h-10 items-center justify-center rounded-md bg-gradient-to-br from-violet-600 to-violet-700 px-4 py-2 text-sm font-extrabold text-white no-underline shadow-lg shadow-violet-600/20 transition hover:from-violet-700 hover:to-violet-600" routerLink="/movie-review/create">Create Review</a>
            <button class="inline-flex min-h-10 items-center justify-center rounded-md border border-violet-500 px-4 py-2 text-sm font-extrabold text-violet-700 transition hover:bg-violet-50 dark:border-violet-400 dark:text-violet-200 dark:hover:bg-violet-950/50" type="button" (click)="logout()">Logout</button>
          } @else {
            <a class="inline-flex min-h-10 items-center justify-center rounded-md border border-violet-500 px-4 py-2 text-sm font-extrabold text-violet-700 no-underline transition hover:bg-violet-50 dark:border-violet-400 dark:text-violet-200 dark:hover:bg-violet-950/50" routerLink="/login">Login</a>
            <a class="inline-flex min-h-10 items-center justify-center rounded-md bg-rose-600 px-4 py-2 text-sm font-extrabold text-white no-underline transition hover:bg-rose-700" routerLink="/register">Register</a>
          }
          <button
            class="inline-flex min-h-10 items-center justify-center rounded-md border border-slate-200 px-4 py-2 text-sm font-extrabold text-slate-700 transition hover:border-violet-300 hover:text-violet-700 dark:border-violet-900 dark:text-violet-100 dark:hover:border-violet-500"
            type="button"
            (click)="theme.toggleTheme()"
          >
            {{ theme.currentTheme() === 'dark' ? 'Light' : 'Dark' }}
          </button>
        </nav>
      </div>
    </header>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class NavbarComponent {
  protected readonly session = inject(AuthSessionStorage);
  protected readonly theme = inject(ThemeService);
  private readonly router = inject(Router);

  logout(): void {
    this.session.clearSession();
    void this.router.navigateByUrl('/movie-reviews');
  }
}
