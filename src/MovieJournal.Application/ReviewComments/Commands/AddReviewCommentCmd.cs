using MovieJournal.Application.Common;
using MovieJournal.Application.Exceptions;
using MovieJournal.Application.MovieReviews;
using MovieJournal.Application.ReviewComments.Mappers;
using MovieJournal.Application.ReviewComments.Requests;
using MovieJournal.Application.ReviewComments.Responses;
using MovieJournal.Domain.Entities;

namespace MovieJournal.Application.ReviewComments.Commands;

public class AddReviewCommentCmd : ICommand<AddReviewCommentRequest, ReviewCommentResponse>
{
    private readonly IMovieReviewsRepository _movieReviewsRepository;
    private readonly IReviewCommentsRepository _reviewCommentsRepository;

    public AddReviewCommentCmd(
        IMovieReviewsRepository movieReviewsRepository,
        IReviewCommentsRepository reviewCommentsRepository)
    {
        _movieReviewsRepository = movieReviewsRepository;
        _reviewCommentsRepository = reviewCommentsRepository;
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

        return ReviewCommentMapper.ToResponse(createdReviewComment);
    }
}
