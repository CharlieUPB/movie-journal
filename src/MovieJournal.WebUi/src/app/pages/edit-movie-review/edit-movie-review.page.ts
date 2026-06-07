import { ChangeDetectionStrategy, Component, inject, input } from '@angular/core';
import { MovieReviewsApi } from '../../data-access/movie-reviews/movie-reviews.api';

@Component({
  selector: 'app-edit-movie-review-page',
  template: `
    <section class="page-panel wide">
      <p class="eyebrow">Edit Review</p>
      <h1>Update your review.</h1>
      <p class="page-copy">Review id: {{ id() }}</p>

      <div class="form-grid">
        <div class="field-placeholder">Movie title</div>
        <div class="field-placeholder">Rating</div>
        <div class="field-placeholder full">Review content</div>
      </div>
    </section>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class EditMovieReviewPage {
  readonly id = input.required<string>();
  private readonly movieReviewsApi = inject(MovieReviewsApi);

  constructor() {
    void this.movieReviewsApi;
  }
}
