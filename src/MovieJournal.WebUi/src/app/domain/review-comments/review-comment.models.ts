export interface ReviewComment {
  id: string;
  movieReviewId: string;
  ownerName: string;
  isOwner: boolean;
  content: string;
  createdAt: string;
  updatedAt: string | null;
}
