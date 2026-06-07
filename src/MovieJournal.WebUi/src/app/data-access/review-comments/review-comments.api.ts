import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiClient } from '../../core/api/api-client';
import {
  AddReviewCommentRequest,
  UpdateReviewCommentRequest,
} from '../../domain/review-comments/review-comment.requests';
import {
  ReviewCommentResponse,
  ReviewCommentsListResponse,
} from '../../domain/review-comments/review-comment.responses';

@Injectable({ providedIn: 'root' })
export class ReviewCommentsApi {
  private readonly api = inject(ApiClient);

  getForMovieReview(movieReviewId: string): Observable<ReviewCommentsListResponse> {
    return this.api.get<ReviewCommentsListResponse>(
      `/api/movie-reviews/${movieReviewId}/comments`,
    );
  }

  add(
    movieReviewId: string,
    request: AddReviewCommentRequest,
  ): Observable<ReviewCommentResponse> {
    return this.api.post<AddReviewCommentRequest, ReviewCommentResponse>(
      `/api/movie-reviews/${movieReviewId}/comments`,
      request,
    );
  }

  update(
    commentId: string,
    request: UpdateReviewCommentRequest,
  ): Observable<ReviewCommentResponse> {
    return this.api.put<UpdateReviewCommentRequest, ReviewCommentResponse>(
      `/api/review-comments/${commentId}`,
      request,
    );
  }

  delete(commentId: string): Observable<void> {
    return this.api.delete(`/api/review-comments/${commentId}`);
  }
}
