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
  templateUrl: './login.page.html',
  styleUrl: './login.page.scss',
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
