import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { AuthSessionStorage } from '../../core/auth/auth-session.storage';
import { UsersApi } from '../../data-access/users/users.api';

@Component({
  selector: 'app-login-page',
  imports: [ReactiveFormsModule, RouterLink],
  template: `
    <section class="auth-page">
      <article class="auth-card">
        <div class="auth-brand">
          <span class="auth-logo">MJ</span>
          <div>
            <p class="eyebrow">Welcome back</p>
            <h1>Movie Review Journal</h1>
          </div>
        </div>

        <form class="auth-form" [formGroup]="form" (ngSubmit)="submit()" novalidate>
          <label class="auth-field">
            <span>Email</span>
            <input
              type="email"
              autocomplete="email"
              placeholder="you@example.com"
              formControlName="email"
              [class.invalid]="showError(email)"
            />
          </label>
          @if (showError(email)) {
            <p class="field-error">{{ emailError() }}</p>
          }

          <label class="auth-field">
            <span>Password</span>
            <div class="password-control">
              <input
                [type]="showPassword() ? 'text' : 'password'"
                autocomplete="current-password"
                placeholder="Enter your password"
                formControlName="password"
                [class.invalid]="showError(password)"
              />
              <button
                class="password-toggle"
                type="button"
                (click)="showPassword.set(!showPassword())"
              >
                {{ showPassword() ? 'Hide' : 'Show' }}
              </button>
            </div>
          </label>
          @if (showError(password)) {
            <p class="field-error">Password is required.</p>
          }

          @if (errorMessage()) {
            <div class="auth-error" role="alert">{{ errorMessage() }}</div>
          }

          <button class="btn btn-primary auth-submit" type="submit" [disabled]="isLoading()">
            {{ isLoading() ? 'Logging in...' : 'Login' }}
          </button>
        </form>

        <p class="auth-switch">
          Do not have an account?
          <a routerLink="/register">Register</a>
        </p>
      </article>
    </section>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LoginPage {
  private readonly UsersApi = inject(UsersApi);
  private readonly session = inject(AuthSessionStorage);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly isLoading = signal(false);
  protected readonly errorMessage = signal<string | null>(null);
  protected readonly showPassword = signal(false);

  readonly form = new FormGroup({
    email: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.email],
    }),
    password: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required],
    }),
  });

  protected get email(): FormControl<string> {
    return this.form.controls.email;
  }

  protected get password(): FormControl<string> {
    return this.form.controls.password;
  }

  submit(): void {
    this.form.markAllAsTouched();
    this.errorMessage.set(null);

    if (this.form.invalid || this.isLoading()) {
      return;
    }

    this.isLoading.set(true);

    this.UsersApi
      .login({
        email: this.email.value,
        password: this.password.value,
      })
      .pipe(
        finalize(() => this.isLoading.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: (response) => {
          this.session.saveSession(response);
          void this.router.navigateByUrl('/movie-reviews/my');
        },
        error: (error: unknown) => this.errorMessage.set(getErrorMessage(error)),
      });
  }

  protected showError(control: FormControl<string>): boolean {
    return control.invalid && (control.touched || control.dirty);
  }

  protected emailError(): string {
    if (this.email.hasError('required')) {
      return 'Email is required.';
    }

    return 'Enter a valid email address.';
  }
}

function getErrorMessage(error: unknown): string {
  if (error instanceof HttpErrorResponse) {
    const problem = error.error as { detail?: string; Detail?: string } | null;
    return problem?.detail ?? problem?.Detail ?? 'Something went wrong. Please try again.';
  }

  return 'Something went wrong. Please try again.';
}
