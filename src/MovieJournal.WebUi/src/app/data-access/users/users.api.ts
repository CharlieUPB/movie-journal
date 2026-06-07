import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiClient } from '../../core/api/api-client';
import { LoginUserRequest, RegisterUserRequest } from '../../domain/users/auth.requests';
import { AuthResponse } from '../../domain/users/auth.responses';

@Injectable({ providedIn: 'root' })
export class UsersApi {
  private readonly api = inject(ApiClient);

  register(request: RegisterUserRequest): Observable<AuthResponse> {
    return this.api.post<RegisterUserRequest, AuthResponse>('/api/users/register', request);
  }

  login(request: LoginUserRequest): Observable<AuthResponse> {
    return this.api.post<LoginUserRequest, AuthResponse>('/api/users/login', request);
  }
}
