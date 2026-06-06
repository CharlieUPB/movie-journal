using MovieJournal.Application.Common;
using MovieJournal.Application.MovieReviews.Mappers;
using MovieJournal.Application.MovieReviews.Requests;
using MovieJournal.Application.MovieReviews.Responses;
using MovieJournal.Application.Exceptions;
using MovieJournal.Domain.Enums;

namespace MovieJournal.Application.MovieReviews.Queries;

public class ListMovieReviewsByUserIdAndStatusQuery : IQuery<ListMovieReviewsByUserIdAndStatusRequest, MovieReviewsListResponse>
{
    private readonly IMovieReviewsRepository _repository;

    public ListMovieReviewsByUserIdAndStatusQuery(IMovieReviewsRepository repository)
    {
        _repository = repository;
    }

    public async Task<MovieReviewsListResponse> Execute(ListMovieReviewsByUserIdAndStatusRequest request)
    {
        if (request.UserId == Guid.Empty)
        {
            throw new UseCaseException("User id is required");
        }

        if (!Enum.IsDefined(typeof(ReviewStatus), request.Status))
        {
            throw new UseCaseException("Review status is invalid");
        }

        var movieReviews = await _repository.ListAsync(new MovieReviewQueryCriteria(request.UserId, request.Status));

        return new MovieReviewsListResponse(MovieReviewMapper.ToResponseList(movieReviews));
    }
}