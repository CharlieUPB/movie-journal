import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, input, output } from '@angular/core';
import { MovieReviewResponse } from '../../../domain/movie-reviews/movie-review.responses';
import { RatingStarsComponent } from '../rating-stars/rating-stars.component';
import { ReviewStatusBadgeComponent } from '../review-status-badge/review-status-badge.component';

@Component({
  selector: 'app-review-card',
  imports: [DatePipe, RatingStarsComponent, ReviewStatusBadgeComponent],
  templateUrl: './review-card.component.html',
  styleUrl: './review-card.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ReviewCardComponent {
  readonly review = input.required<MovieReviewResponse>();
  readonly showActions = input(false);
  readonly actionDisabled = input(false);

  readonly view = output<string>();
  readonly edit = output<string>();
  readonly publish = output<string>();
  readonly archive = output<string>();
  readonly remove = output<string>();

  protected readonly contentPreview = computed(() => {
    const content = this.review().reviewContent.trim();
    return content.length > 170 ? `${content.slice(0, 167)}...` : content;
  });

  protected open(): void {
    this.view.emit(this.review().id);
  }
}
