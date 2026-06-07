using MovieJournal.Application.Common;
using MovieJournal.Application.Exceptions;
using MovieJournal.Application.MovieReviews;
using MovieJournal.Application.ReviewComments.Mappers;
using MovieJournal.Application.ReviewComments.Requests;
using MovieJournal.Application.ReviewComments.Responses;
using MovieJournal.Application.Users;
using MovieJournal.Domain.Entities;

namespace MovieJournal.Application.ReviewComments.Commands;

public class AddReviewCommentCmd : ICommand<AddReviewCommentRequest, ReviewCommentResponse>
{
    private readonly IMovieReviewsRepository _movieReviewsRepository;
    private readonly IReviewCommentsRepository _reviewCommentsRepository;
    private readonly IUserRepository _userRepository;

    public AddReviewCommentCmd(
        IMovieReviewsRepository movieReviewsRepository,
        IReviewCommentsRepository reviewCommentsRepository,
        IUserRepository userRepository)
    {
        _movieReviewsRepository = movieReviewsRepository;
        _reviewCommentsRepository = reviewCommentsRepository;
        _userRepository = userRepository;
    }

    public async Task<ReviewCommentResponse> Execute(AddReviewCommentRequest request)
    {
        if (request.UserId == Guid.Empty)
        {
            throw new UseCaseException("User must be authenticated to add comments");
        }

        var movieReview = await _movieReviewsRepository.GetByIdAsync(request.MovieReviewId);

        if (movieReview is null)
        {
            throw new UseCaseException("Movie review was not found");
        }

        movieReview.EnsureCanReceiveComments();

        var reviewComment = ReviewComment.Create(
            request.MovieReviewId,
            request.UserId,
            request.Content);

        var createdReviewComment = await _reviewCommentsRepository.CreateAsync(reviewComment);
        var owner = await _userRepository.GetByIdAsync(createdReviewComment.UserId);

        return ReviewCommentMapper.ToResponse(
            createdReviewComment,
            owner?.DisplayName ?? "Unknown user",
            true);
    }
}
