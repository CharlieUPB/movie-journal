export interface ReviewComment {
  id: string;
  movieReviewId: string;
  userId: string;
  content: string;
  createdAt: string;
  updatedAt: string | null;
}
