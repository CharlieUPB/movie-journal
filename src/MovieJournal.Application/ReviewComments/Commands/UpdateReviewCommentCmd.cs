using MovieJournal.Application.Common;
using MovieJournal.Application.Exceptions;
using MovieJournal.Application.MovieReviews;
using MovieJournal.Application.ReviewComments.Mappers;
using MovieJournal.Application.ReviewComments.Requests;
using MovieJournal.Application.ReviewComments.Responses;
using MovieJournal.Application.Users;

namespace MovieJournal.Application.ReviewComments.Commands;

public class UpdateReviewCommentCmd : ICommand<UpdateReviewCommentRequest, ReviewCommentResponse>
{
    private readonly IMovieReviewsRepository _movieReviewsRepository;
    private readonly IReviewCommentsRepository _reviewCommentsRepository;
    private readonly IUserRepository _userRepository;

    public UpdateReviewCommentCmd(
        IMovieReviewsRepository movieReviewsRepository,
        IReviewCommentsRepository reviewCommentsRepository,
        IUserRepository userRepository)
    {
        _movieReviewsRepository = movieReviewsRepository;
        _reviewCommentsRepository = reviewCommentsRepository;
        _userRepository = userRepository;
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
        var owner = await _userRepository.GetByIdAsync(updatedReviewComment.UserId);

        return ReviewCommentMapper.ToResponse(
            updatedReviewComment,
            owner?.DisplayName ?? "Unknown user",
            true);
    }
}
