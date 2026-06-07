import { Injectable, signal } from '@angular/core';
import { AuthResponse } from '../../domain/users/auth.responses';
import { AuthenticatedUser } from './auth.models';

const tokenKey = 'movie-journal.auth.token';
const userKey = 'movie-journal.auth.user';

@Injectable({ providedIn: 'root' })
export class AuthSessionStorage {
  readonly user = signal<AuthenticatedUser | null>(this.getUser());

  saveSession(response: AuthResponse): void {
    const user: AuthenticatedUser = {
      userId: response.userId,
      displayName: response.displayName,
      email: response.email,
    };

    localStorage.setItem(tokenKey, response.token);
    localStorage.setItem(userKey, JSON.stringify(user));
    this.user.set(user);
  }

  getToken(): string | null {
    return localStorage.getItem(tokenKey);
  }

  getUser(): AuthenticatedUser | null {
    const rawUser = localStorage.getItem(userKey);

    if (!rawUser) {
      return null;
    }

    try {
      return JSON.parse(rawUser) as AuthenticatedUser;
    } catch {
      return null;
    }
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  clearSession(): void {
    localStorage.removeItem(tokenKey);
    localStorage.removeItem(userKey);
    this.user.set(null);
  }
}
