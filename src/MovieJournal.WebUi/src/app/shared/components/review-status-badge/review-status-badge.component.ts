import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { ReviewStatus } from '../../../domain/movie-reviews/movie-review.models';

@Component({
  selector: 'app-review-status-badge',
  templateUrl: './review-status-badge.component.html',
  styleUrl: './review-status-badge.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ReviewStatusBadgeComponent {
  readonly status = input.required<ReviewStatus>();
}
