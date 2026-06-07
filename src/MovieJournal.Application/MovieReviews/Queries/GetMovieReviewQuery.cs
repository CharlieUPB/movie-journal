using MovieJournal.Application.Common;
using MovieJournal.Application.Exceptions;
using MovieJournal.Application.MovieReviews.Mappers;
using MovieJournal.Application.MovieReviews.Requests;
using MovieJournal.Application.MovieReviews.Responses;
using MovieJournal.Domain.Enums;

namespace MovieJournal.Application.MovieReviews.Queries;

public class GetMovieReviewQuery : IQuery<GetMovieReviewRequest, MovieReviewResponse>
{
    private readonly IMovieReviewsRepository _repository;

    public GetMovieReviewQuery(IMovieReviewsRepository repository)
    {
        _repository = repository;
    }

    public async Task<MovieReviewResponse> Execute(GetMovieReviewRequest request)
    {
        var movieReview = await _repository.GetByIdAsync(request.MovieReviewId);

        if (movieReview is null)
        {
            throw new UseCaseException("Movie review was not found");
        }

        var isOwner = request.UserId.HasValue && movieReview.UserId == request.UserId.Value;
        var isPublished = movieReview.Status == ReviewStatus.Published;

        if (!isPublished && !isOwner)
        {
            throw new UseCaseException("You are not allowed to view this movie review");
        }

        return MovieReviewMapper.ToResponse(movieReview);
    }
}
