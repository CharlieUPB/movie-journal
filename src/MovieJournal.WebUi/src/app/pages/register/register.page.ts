import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import {
  AbstractControl,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { AuthSessionStorage } from '../../core/auth/auth-session.storage';
import { UsersApi } from '../../data-access/users/users.api';

@Component({
  selector: 'app-register-page',
  imports: [ReactiveFormsModule, RouterLink],
  template: `
    <section class="auth-page">
      <article class="auth-card">
        <div class="auth-brand">
          <span class="auth-logo">MJ</span>
          <div>
            <p class="eyebrow">Create account</p>
            <h1>Movie Review Journal</h1>
          </div>
        </div>

        <form class="auth-form" [formGroup]="form" (ngSubmit)="submit()" novalidate>
          <label class="auth-field">
            <span>Display name</span>
            <input
              type="text"
              autocomplete="name"
              placeholder="Carlos"
              formControlName="displayName"
              [class.invalid]="showError(displayName)"
            />
          </label>
          @if (showError(displayName)) {
            <p class="field-error">Display name is required.</p>
          }

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
                autocomplete="new-password"
                placeholder="At least 6 characters"
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
            <p class="field-error">{{ passwordError() }}</p>
          }

          <label class="auth-field">
            <span>Confirm password</span>
            <input
              [type]="showConfirmPassword() ? 'text' : 'password'"
              autocomplete="new-password"
              placeholder="Repeat your password"
              formControlName="confirmPassword"
              [class.invalid]="showError(confirmPassword) || showPasswordMismatch()"
            />
          </label>
          <button
            class="password-secondary-toggle"
            type="button"
            (click)="showConfirmPassword.set(!showConfirmPassword())"
          >
            {{ showConfirmPassword() ? 'Hide confirmation' : 'Show confirmation' }}
          </button>
          @if (showError(confirmPassword)) {
            <p class="field-error">Confirm password is required.</p>
          } @else if (showPasswordMismatch()) {
            <p class="field-error">Passwords must match.</p>
          }

          @if (errorMessage()) {
            <div class="auth-error" role="alert">{{ errorMessage() }}</div>
          }

          <button class="btn btn-primary auth-submit" type="submit" [disabled]="isLoading()">
            {{ isLoading() ? 'Creating account...' : 'Register' }}
          </button>
        </form>

        <p class="auth-switch">
          Already have an account?
          <a routerLink="/login">Login</a>
        </p>
      </article>
    </section>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RegisterPage {
  private readonly UsersApi = inject(UsersApi);
  private readonly session = inject(AuthSessionStorage);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly isLoading = signal(false);
  protected readonly errorMessage = signal<string | null>(null);
  protected readonly showPassword = signal(false);
  protected readonly showConfirmPassword = signal(false);

  readonly form = new FormGroup(
    {
      displayName: new FormControl('', {
        nonNullable: true,
        validators: [Validators.required],
      }),
      email: new FormControl('', {
        nonNullable: true,
        validators: [Validators.required, Validators.email],
      }),
      password: new FormControl('', {
        nonNullable: true,
        validators: [Validators.required, Validators.minLength(6)],
      }),
      confirmPassword: new FormControl('', {
        nonNullable: true,
        validators: [Validators.required],
      }),
    },
    { validators: passwordMatchValidator },
  );

  protected get displayName(): FormControl<string> {
    return this.form.controls.displayName;
  }

  protected get email(): FormControl<string> {
    return this.form.controls.email;
  }

  protected get password(): FormControl<string> {
    return this.form.controls.password;
  }

  protected get confirmPassword(): FormControl<string> {
    return this.form.controls.confirmPassword;
  }

  submit(): void {
    this.form.markAllAsTouched();
    this.errorMessage.set(null);

    if (this.form.invalid || this.isLoading()) {
      return;
    }

    this.isLoading.set(true);

    this.UsersApi
      .register({
        displayName: this.displayName.value,
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

  protected showPasswordMismatch(): boolean {
    return (
      this.form.hasError('passwordMismatch') &&
      (this.confirmPassword.touched || this.confirmPassword.dirty)
    );
  }

  protected emailError(): string {
    if (this.email.hasError('required')) {
      return 'Email is required.';
    }

    return 'Enter a valid email address.';
  }

  protected passwordError(): string {
    if (this.password.hasError('required')) {
      return 'Password is required.';
    }

    return 'Password must be at least 6 characters.';
  }
}

function passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
  const password = control.get('password')?.value as string | undefined;
  const confirmPassword = control.get('confirmPassword')?.value as string | undefined;

  return password && confirmPassword && password !== confirmPassword
    ? { passwordMismatch: true }
    : null;
}

function getErrorMessage(error: unknown): string {
  if (error instanceof HttpErrorResponse) {
    const problem = error.error as { detail?: string; Detail?: string } | null;
    return problem?.detail ?? problem?.Detail ?? 'Something went wrong. Please try again.';
  }

  return 'Something went wrong. Please try again.';
}
