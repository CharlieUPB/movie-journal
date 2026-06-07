import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthSessionStorage } from './auth-session.storage';

export const authGuard: CanActivateFn = () => {
  const session = inject(AuthSessionStorage);
  const router = inject(Router);

  return session.isLoggedIn() ? true : router.createUrlTree(['/login']);
};
