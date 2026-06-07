import { Routes } from '@angular/router';
import { authGuard, guestGuard } from './core/auth/auth.guard';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'movie-reviews',
  },
  {
    path: 'login',
    canActivate: [guestGuard],
    loadComponent: () => import('./pages/login/login.page').then((m) => m.LoginPage),
  },
  {
    path: 'register',
    canActivate: [guestGuard],
    loadComponent: () => import('./pages/register/register.page').then((m) => m.RegisterPage),
  },
  {
    path: 'movie-reviews/my',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/my-reviews/my-reviews.page').then((m) => m.MyReviewsPage),
  },
  {
    path: 'movie-reviews',
    loadComponent: () =>
      import('./pages/movie-reviews/movie-reviews.page').then((m) => m.MovieReviewsPage),
  },
  {
    path: 'movie-review/create',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/create-movie-review/create-movie-review.page').then(
        (m) => m.CreateMovieReviewPage,
      ),
  },
  {
    path: 'movie-review/:id/edit',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/edit-movie-review/edit-movie-review.page').then(
        (m) => m.EditMovieReviewPage,
      ),
  },
  {
    path: 'movie-review/:id',
    loadComponent: () =>
      import('./pages/movie-review-details/movie-review-details.page').then(
        (m) => m.MovieReviewDetailsPage,
      ),
  },
  {
    path: '**',
    redirectTo: 'movie-reviews',
  },
];
