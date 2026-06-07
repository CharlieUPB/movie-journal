using MovieJournal.Application.Common;
using MovieJournal.Application.Exceptions;
using MovieJournal.Application.MovieReviews;
using MovieJournal.Application.ReviewComments.Mappers;
using MovieJournal.Application.ReviewComments.Requests;
using MovieJournal.Application.ReviewComments.Responses;

namespace MovieJournal.Application.ReviewComments.Commands;

public class UpdateReviewCommentCmd : ICommand<UpdateReviewCommentRequest, ReviewCommentResponse>
{
    private readonly IMovieReviewsRepository _movieReviewsRepository;
    private readonly IReviewCommentsRepository _reviewCommentsRepository;

    public UpdateReviewCommentCmd(
        IMovieReviewsRepository movieReviewsRepository,
        IReviewCommentsRepository reviewCommentsRepository)
    {
        _movieReviewsRepository = movieReviewsRepository;
        _reviewCommentsRepository = reviewCommentsRepository;
    }

    public async Task<ReviewCommentResponse> Execute(UpdateReviewCommentRequest request)
    {
        var reviewComment = await _reviewCommentsRepository.GetByIdAsync(request.CommentId);

        if (reviewComment is null || reviewComment.IsDeleted == true)
        {
            throw new UseCaseException("Review comment was not found");
        }

        if (reviewComment.UserId != request.UserId)
        {
            throw new UseCaseException("You are not allowed to update this review comment");
        }

        var movieReview = await _movieReviewsRepository.GetByIdAsync(reviewComment.MovieReviewId);

        if (movieReview is null)
        {
            throw new UseCaseException("Movie review was not found");
        }

        movieReview.EnsureCanReceiveComments();

        reviewComment.UpdateComment(request.Content);

        var updatedReviewComment = await _reviewCommentsRepository.UpdateAsync(reviewComment);

        return ReviewCommentMapper.ToResponse(updatedReviewComment);
    }
}
