import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, DestroyRef, OnInit, computed, inject, input, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { AuthSessionStorage } from '../../core/auth/auth-session.storage';
import { MovieReviewsApi } from '../../data-access/movie-reviews/movie-reviews.api';
import { ReviewCommentsApi } from '../../data-access/review-comments/review-comments.api';
import { MovieReviewResponse } from '../../domain/movie-reviews/movie-review.responses';
import { ReviewCommentResponse } from '../../domain/review-comments/review-comment.responses';
import { CommentFormComponent } from '../../shared/components/comment-form/comment-form.component';
import { CommentsListComponent } from '../../shared/components/comments-list/comments-list.component';
import { RatingStarsComponent } from '../../shared/components/rating-stars/rating-stars.component';
import { ReviewStatusBadgeComponent } from '../../shared/components/review-status-badge/review-status-badge.component';
import { StateMessageComponent } from '../../shared/components/state-message/state-message.component';

@Component({
  selector: 'app-movie-review-details-page',
  imports: [
    DatePipe,
    RouterLink,
    RatingStarsComponent,
    ReviewStatusBadgeComponent,
    StateMessageComponent,
    CommentsListComponent,
    CommentFormComponent,
  ],
  templateUrl: './movie-review-details.page.html',
  styleUrl: './movie-review-details.page.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MovieReviewDetailsPage implements OnInit {
  readonly id = input.required<string>();

  private readonly movieReviewsApi = inject(MovieReviewsApi);
  private readonly reviewCommentsApi = inject(ReviewCommentsApi);
  protected readonly session = inject(AuthSessionStorage);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly review = signal<MovieReviewResponse | null>(null);
  protected readonly comments = signal<ReviewCommentResponse[]>([]);
  protected readonly isLoadingReview = signal(true);
  protected readonly isLoadingComments = signal(true);
  protected readonly errorMessage = signal<string | null>(null);
  protected readonly commentErrorMessage = signal<string | null>(null);
  protected readonly isSubmittingComment = signal(false);

  protected readonly canAddComment = computed(
    () => this.review()?.status === 'Published' && this.session.isLoggedIn(),
  );

  ngOnInit(): void {
    this.loadReview();
    this.loadComments();
  }

  protected addComment(content: string): void {
    this.commentErrorMessage.set(null);
    this.isSubmittingComment.set(true);

    this.reviewCommentsApi
      .add(this.id(), { content })
      .pipe(
        finalize(() => this.isSubmittingComment.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: (comment) => this.comments.update((comments) => [...comments, comment]),
        error: () => this.commentErrorMessage.set('Comment could not be added. Please try again.'),
      });
  }

  private loadReview(): void {
    this.isLoadingReview.set(true);
    this.errorMessage.set(null);

    this.movieReviewsApi
      .getById(this.id())
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (review) => {
          this.review.set(review);
          this.isLoadingReview.set(false);
        },
        error: () => {
          this.errorMessage.set('Movie review could not be loaded. Please try again.');
          this.isLoadingReview.set(false);
        },
      });
  }

  private loadComments(): void {
    this.isLoadingComments.set(true);

    this.reviewCommentsApi
      .getForMovieReview(this.id())
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (response) => {
          this.comments.set(response.comments);
          this.isLoadingComments.set(false);
        },
        error: () => {
          this.commentErrorMessage.set('Comments could not be loaded. Please try again.');
          this.isLoadingComments.set(false);
        },
      });
  }
}
