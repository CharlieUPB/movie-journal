import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { MovieReviewsApi } from '../../data-access/movie-reviews/movie-reviews.api';

@Component({
  selector: 'app-create-movie-review-page',
  template: `
    <section class="page-panel wide">
      <p class="eyebrow">Create Review</p>
      <h1>Start a movie review.</h1>
      <p class="page-copy">This protected page is ready for movie and review form fields.</p>

      <div class="form-grid">
        <div class="field-placeholder">Movie title</div>
        <div class="field-placeholder">Release year</div>
        <div class="field-placeholder full">Your review</div>
      </div>
    </section>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CreateMovieReviewPage {
  private readonly movieReviewsApi = inject(MovieReviewsApi);

  constructor() {
    void this.movieReviewsApi;
  }
}
