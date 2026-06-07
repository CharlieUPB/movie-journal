import { ChangeDetectionStrategy, Component, DestroyRef, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Router, RouterLink } from '@angular/router';
import { MovieReviewsApi } from '../../data-access/movie-reviews/movie-reviews.api';
import { MovieReviewResponse } from '../../domain/movie-reviews/movie-review.responses';
import { ReviewCardComponent } from '../../shared/components/review-card/review-card.component';
import { StateMessageComponent } from '../../shared/components/state-message/state-message.component';

@Component({
  selector: 'app-movie-reviews-page',
  imports: [RouterLink, ReviewCardComponent, StateMessageComponent],
  templateUrl: './movie-reviews.page.html',
  styleUrl: './movie-reviews.page.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MovieReviewsPage {
  private readonly movieReviewsApi = inject(MovieReviewsApi);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly isLoading = signal(true);
  protected readonly errorMessage = signal<string | null>(null);
  protected readonly reviews = signal<MovieReviewResponse[]>([]);
  protected readonly searchTerm = signal('');

  protected readonly filteredReviews = computed(() => {
    const term = this.searchTerm().trim().toLowerCase();

    if (!term) {
      return this.reviews();
    }

    return this.reviews().filter((review) => review.movieTitle.toLowerCase().includes(term));
  });

  constructor() {
    this.loadReviews();
  }

  protected updateSearch(value: string): void {
    this.searchTerm.set(value);
  }

  protected viewReview(id: string): void {
    void this.router.navigate(['/movie-review', id]);
  }

  private loadReviews(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.movieReviewsApi
      .getPublished()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (response) => {
          this.reviews.set(response.reviews);
          this.isLoading.set(false);
        },
        error: () => {
          this.errorMessage.set('Published reviews could not be loaded. Please try again.');
          this.isLoading.set(false);
        },
      });
  }
}
