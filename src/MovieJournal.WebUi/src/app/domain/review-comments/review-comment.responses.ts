import { ReviewComment } from './review-comment.models';

export interface ReviewCommentResponse extends ReviewComment {}

export interface ReviewCommentsListResponse {
  comments: ReviewCommentResponse[];
}
