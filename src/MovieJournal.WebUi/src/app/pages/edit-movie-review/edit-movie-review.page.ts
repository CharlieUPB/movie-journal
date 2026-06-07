import { HttpErrorResponse } from '@angular/common/http';
import {
  ChangeDetectionStrategy,
  Component,
  DestroyRef,
  OnInit,
  computed,
  inject,
  input,
  signal,
} from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Router, RouterLink } from '@angular/router';
import { Observable, finalize } from 'rxjs';
import { MovieReviewsApi } from '../../data-access/movie-reviews/movie-reviews.api';
import { MovieReviewResponse } from '../../domain/movie-reviews/movie-review.responses';
import {
  ReviewFormComponent,
  ReviewFormValue,
} from '../../shared/components/review-form/review-form.component';
import { ReviewStatusBadgeComponent } from '../../shared/components/review-status-badge/review-status-badge.component';
import { StateMessageComponent } from '../../shared/components/state-message/state-message.component';

@Component({
  selector: 'app-edit-movie-review-page',
  imports: [RouterLink, ReviewFormComponent, ReviewStatusBadgeComponent, StateMessageComponent],
  templateUrl: './edit-movie-review.page.html',
  styleUrl: './edit-movie-review.page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class EditMovieReviewPage implements OnInit {
  readonly id = input.required<string>();

  private readonly movieReviewsApi = inject(MovieReviewsApi);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly review = signal<MovieReviewResponse | null>(null);
  protected readonly isLoading = signal(true);
  protected readonly isSaving = signal(false);
  protected readonly errorMessage = signal<string | null>(null);
  protected readonly successMessage = signal<string | null>(null);
  protected readonly actionName = signal<string | null>(null);

  protected readonly formValue = computed<ReviewFormValue | null>(() => {
    const review = this.review();

    if (!review) {
      return null;
    }

    return {
      movieTitle: review.movieTitle,
      movieReleaseYear: review.movieReleaseYear,
      reviewTitle: review.reviewTitle,
      reviewContent: review.reviewContent,
      reviewRating: review.reviewRating,
    };
  });

  protected readonly isArchived = computed(() => this.review()?.status === 'Archived');
  protected readonly isBusy = computed(() => this.isSaving() || this.actionName() !== null);

  ngOnInit(): void {
    this.loadReview();
  }

  protected saveReview(value: ReviewFormValue): void {
    if (this.isArchived()) {
      return;
    }

    this.isSaving.set(true);
    this.errorMessage.set(null);
    this.successMessage.set(null);

    this.movieReviewsApi
      .update(this.id(), value)
      .pipe(
        finalize(() => this.isSaving.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: (review) => {
          this.review.set(review);
          this.successMessage.set('Review changes were saved.');
        },
        error: (error: unknown) => this.errorMessage.set(getErrorMessage(error)),
      });
  }

  protected publishReview(): void {
    this.runReviewAction('Publishing', () => this.movieReviewsApi.publish(this.id()), 'Review was published.');
  }

  protected archiveReview(): void {
    this.runReviewAction('Archiving', () => this.movieReviewsApi.archive(this.id()), 'Review was archived.');
  }

  protected deleteReview(): void {
    if (!window.confirm('Delete this review? This cannot be undone.')) {
      return;
    }

    this.actionName.set('Deleting');
    this.errorMessage.set(null);
    this.successMessage.set(null);

    this.movieReviewsApi
      .delete(this.id())
      .pipe(
        finalize(() => this.actionName.set(null)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: () => void this.router.navigate(['/movie-reviews/my']),
        error: (error: unknown) => this.errorMessage.set(getErrorMessage(error)),
      });
  }

  private loadReview(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.movieReviewsApi
      .getById(this.id())
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (review) => {
          this.review.set(review);
          this.isLoading.set(false);
        },
        error: (error: unknown) => {
          this.errorMessage.set(getErrorMessage(error));
          this.isLoading.set(false);
        },
      });
  }

  private runReviewAction(
    actionName: string,
    action: () => Observable<MovieReviewResponse>,
    successMessage: string,
  ): void {
    this.actionName.set(actionName);
    this.errorMessage.set(null);
    this.successMessage.set(null);

    action()
      .pipe(
        finalize(() => this.actionName.set(null)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: (review) => {
          this.review.set(review);
          this.successMessage.set(successMessage);
        },
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
