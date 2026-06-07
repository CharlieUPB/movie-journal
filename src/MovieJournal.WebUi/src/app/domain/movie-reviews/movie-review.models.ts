export type ReviewStatus = 'Draft' | 'Published' | 'Archived';

export interface MovieReview {
  id: string;
  userId: string;
  movieTitle: string;
  movieReleaseYear: number | null;
  reviewTitle: string;
  reviewContent: string;
  reviewRating: number;
  status: ReviewStatus;
  createdAt: string;
  updatedAt: string | null;
}
