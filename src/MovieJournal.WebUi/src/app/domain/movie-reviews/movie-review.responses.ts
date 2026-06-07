import { MovieReview } from './movie-review.models';

export interface MovieReviewResponse extends MovieReview {}

export interface MovieReviewsListResponse {
  reviews: MovieReviewResponse[];
}
