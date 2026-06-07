using MovieJournal.Application.Common;
using MovieJournal.Application.Exceptions;
using MovieJournal.Application.MovieReviews;
using MovieJournal.Application.ReviewComments.Mappers;
using MovieJournal.Application.ReviewComments.Requests;
using MovieJournal.Application.ReviewComments.Responses;
using MovieJournal.Domain.Enums;

namespace MovieJournal.Application.ReviewComments.Queries;

public class ListReviewCommentsQuery : IQuery<ListReviewCommentsRequest, ReviewCommentsListResponse>
{
    private readonly IMovieReviewsRepository _movieReviewsRepository;
    private readonly IReviewCommentsRepository _reviewCommentsRepository;

    public ListReviewCommentsQuery(
        IMovieReviewsRepository movieReviewsRepository,
        IReviewCommentsRepository reviewCommentsRepository)
    {
        _movieReviewsRepository = movieReviewsRepository;
        _reviewCommentsRepository = reviewCommentsRepository;
    }

    public async Task<ReviewCommentsListResponse> Execute(ListReviewCommentsRequest request)
    {
        var movieReview = await _movieReviewsRepository.GetByIdAsync(request.MovieReviewId);

        if (movieReview is null)
        {
            throw new UseCaseException("Movie review was not found");
        }

        var isOwner = request.UserId.HasValue && movieReview.UserId == request.UserId.Value;

        if (movieReview.Status != ReviewStatus.Published && !isOwner)
        {
            throw new UseCaseException("You are not allowed to view these review comments");
        }

        var reviewComments = await _reviewCommentsRepository.GetByMovieReviewIdAsync(request.MovieReviewId);
        var visibleReviewComments = reviewComments
            .Where(reviewComment => reviewComment.IsDeleted != true)
            .ToList();

        return new ReviewCommentsListResponse(ReviewCommentMapper.ToResponseList(visibleReviewComments));
    }
}
