import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MovieReviewsApi } from '../../data-access/movie-reviews/movie-reviews.api';

@Component({
  selector: 'app-movie-reviews-page',
  imports: [RouterLink],
  template: `
    <section class="page-header">
      <div>
        <p class="eyebrow">Published Reviews</p>
        <h1>Discover thoughtful takes from movie lovers.</h1>
      </div>
      <a class="btn btn-primary" routerLink="/movie-review/create">Create Review</a>
    </section>

    <section class="content-grid">
      @for (review of placeholderReviews(); track review.title) {
        <article class="card">
          <div class="card-heading">
            <h2>{{ review.title }}</h2>
            <span class="rating">{{ review.rating }}</span>
          </div>
          <p>{{ review.summary }}</p>
          <a class="text-link" routerLink="/movie-review/placeholder">View details</a>
        </article>
      }
    </section>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MovieReviewsPage {
  private readonly movieReviewsApi = inject(MovieReviewsApi);
  protected readonly placeholderReviews = signal([
    {
      title: 'Inception',
      rating: '5.0',
      summary: 'A polished placeholder for published review cards.',
    },
    {
      title: 'The Shawshank Redemption',
      rating: '4.0',
      summary: 'The API client is available when this page becomes data-driven.',
    },
  ]);

  constructor() {
    void this.movieReviewsApi;
  }
}
