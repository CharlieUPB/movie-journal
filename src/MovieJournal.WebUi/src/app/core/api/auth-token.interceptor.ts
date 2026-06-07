import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthSessionStorage } from '../auth/auth-session.storage';

export const authTokenInterceptor: HttpInterceptorFn = (request, next) => {
  const token = inject(AuthSessionStorage).getToken();

  if (!token) {
    return next(request);
  }

  return next(
    request.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`,
      },
    }),
  );
};
