import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Router, RouterLink } from '@angular/router';
import { Observable, finalize } from 'rxjs';
import { MovieReviewsApi } from '../../data-access/movie-reviews/movie-reviews.api';
import { ReviewStatus } from '../../domain/movie-reviews/movie-review.models';
import { MovieReviewResponse } from '../../domain/movie-reviews/movie-review.responses';
import { ReviewCardComponent } from '../../shared/components/review-card/review-card.component';
import { StateMessageComponent } from '../../shared/components/state-message/state-message.component';

type ReviewFilter = 'All' | ReviewStatus;

@Component({
  selector: 'app-my-reviews-page',
  imports: [RouterLink, ReviewCardComponent, StateMessageComponent],
  templateUrl: './my-reviews.page.html',
  styleUrl: './my-reviews.page.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MyReviewsPage {
  private readonly movieReviewsApi = inject(MovieReviewsApi);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly filters: ReviewFilter[] = ['All', 'Draft', 'Published', 'Archived'];
  protected readonly selectedFilter = signal<ReviewFilter>('All');
  protected readonly reviews = signal<MovieReviewResponse[]>([]);
  protected readonly isLoading = signal(true);
  protected readonly errorMessage = signal<string | null>(null);
  protected readonly actionReviewId = signal<string | null>(null);

  constructor() {
    this.loadReviews();
  }

  protected selectFilter(filter: ReviewFilter): void {
    if (this.selectedFilter() === filter) {
      return;
    }

    this.selectedFilter.set(filter);
    this.loadReviews();
  }

  protected viewReview(id: string): void {
    void this.router.navigate(['/movie-review', id]);
  }

  protected editReview(id: string): void {
    void this.router.navigate(['/movie-review', id, 'edit']);
  }

  protected publishReview(id: string): void {
    this.runAction(id, () => this.movieReviewsApi.publish(id));
  }

  protected archiveReview(id: string): void {
    this.runAction(id, () => this.movieReviewsApi.archive(id));
  }

  protected deleteReview(id: string): void {
    if (!window.confirm('Delete this review? This cannot be undone.')) {
      return;
    }

    this.runAction(id, () => this.movieReviewsApi.delete(id));
  }

  private loadReviews(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    const selectedFilter = this.selectedFilter();
    const status = selectedFilter === 'All' ? undefined : selectedFilter;

    this.movieReviewsApi
      .getMine({ status })
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (response) => {
          this.reviews.set(response.reviews);
          this.isLoading.set(false);
        },
        error: () => {
          this.errorMessage.set('Your reviews could not be loaded. Please try again.');
          this.isLoading.set(false);
        },
      });
  }

  private runAction(id: string, action: () => Observable<unknown>): void {
    this.actionReviewId.set(id);
    this.errorMessage.set(null);

    action()
      .pipe(
        finalize(() => this.actionReviewId.set(null)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: () => this.loadReviews(),
        error: () => this.errorMessage.set('The review action could not be completed.'),
      });
  }
}
