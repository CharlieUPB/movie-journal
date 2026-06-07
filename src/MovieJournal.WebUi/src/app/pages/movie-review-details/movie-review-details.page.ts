import { ChangeDetectionStrategy, Component, inject, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MovieReviewsApi } from '../../data-access/movie-reviews/movie-reviews.api';
import { ReviewCommentsApi } from '../../data-access/review-comments/review-comments.api';

@Component({
  selector: 'app-movie-review-details-page',
  imports: [RouterLink],
  template: `
    <section class="page-panel wide">
      <p class="eyebrow">Review Details</p>
      <h1>Movie review</h1>
      <p class="page-copy">Review id: {{ id() }}</p>

      <div class="placeholder-card">
        <span>Comments API</span>
        <strong>Ready for review comments</strong>
      </div>

      <a class="text-link" routerLink="/movie-reviews">Back to reviews</a>
    </section>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MovieReviewDetailsPage {
  readonly id = input.required<string>();
  private readonly movieReviewsApi = inject(MovieReviewsApi);
  private readonly reviewCommentsApi = inject(ReviewCommentsApi);

  constructor() {
    void this.movieReviewsApi;
    void this.reviewCommentsApi;
  }
}
