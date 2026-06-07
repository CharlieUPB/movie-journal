import { ReviewStatus } from './movie-review.models';

export interface CreateMovieReviewRequest {
  movieTitle: string;
  reviewTitle: string;
  reviewContent: string;
  reviewRating: number;
  movieReleaseYear: number | null;
}

export interface UpdateMovieReviewRequest extends CreateMovieReviewRequest {}

export interface ListMyMovieReviewsOptions {
  status?: ReviewStatus;
}
