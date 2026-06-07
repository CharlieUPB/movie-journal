import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Router, RouterLink } from '@angular/router';
import { finalize, switchMap } from 'rxjs';
import { MovieReviewsApi } from '../../data-access/movie-reviews/movie-reviews.api';
import {
  ReviewFormComponent,
  ReviewFormValue,
} from '../../shared/components/review-form/review-form.component';
import { StateMessageComponent } from '../../shared/components/state-message/state-message.component';

@Component({
  selector: 'app-create-movie-review-page',
  imports: [RouterLink, ReviewFormComponent, StateMessageComponent],
  templateUrl: './create-movie-review.page.html',
  styleUrl: './create-movie-review.page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CreateMovieReviewPage {
  private readonly movieReviewsApi = inject(MovieReviewsApi);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly isSaving = signal(false);
  protected readonly errorMessage = signal<string | null>(null);

  protected saveDraft(value: ReviewFormValue): void {
    this.isSaving.set(true);
    this.errorMessage.set(null);

    this.movieReviewsApi
      .create(value)
      .pipe(
        finalize(() => this.isSaving.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: (review) => void this.router.navigate(['/movie-review', review.id]),
        error: (error: unknown) => this.errorMessage.set(getErrorMessage(error)),
      });
  }

  protected saveAndPublish(value: ReviewFormValue): void {
    this.isSaving.set(true);
    this.errorMessage.set(null);

    this.movieReviewsApi
      .create(value)
      .pipe(
        switchMap((review) => this.movieReviewsApi.publish(review.id)),
        finalize(() => this.isSaving.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: (review) => void this.router.navigate(['/movie-review', review.id]),
        error: (error: unknown) => this.errorMessage.set(getErrorMessage(error)),
      });
  }
}

function getErrorMessage(error: unknown): string {
  if (error instanceof HttpErrorResponse) {
    const problem = error.error as { detail?: string; title?: string; Detail?: string; Title?: string } | null;
    return (
      problem?.detail ??
      problem?.Detail ??
      problem?.title ??
      problem?.Title ??
      'Something went wrong. Please try again.'
    );
  }

  return 'Something went wrong. Please try again.';
}
