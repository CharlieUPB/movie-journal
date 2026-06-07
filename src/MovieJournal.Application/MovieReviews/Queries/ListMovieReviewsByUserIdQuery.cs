using MovieJournal.Application.Common;
using MovieJournal.Application.Exceptions;
using MovieJournal.Application.MovieReviews.Mappers;
using MovieJournal.Application.MovieReviews.Requests;
using MovieJournal.Application.MovieReviews.Responses;

namespace MovieJournal.Application.MovieReviews.Queries;

public class ListMovieReviewsByUserIdQuery : IQuery<ListMovieReviewsByUserIdRequest, MovieReviewsListResponse>
{
    private readonly IMovieReviewsRepository _repository;

    public ListMovieReviewsByUserIdQuery(IMovieReviewsRepository repository)
    {
        _repository = repository;
    }

    public async Task<MovieReviewsListResponse> Execute(ListMovieReviewsByUserIdRequest request)
    {
        if (request.UserId == Guid.Empty)
        {
            throw new UseCaseException("User id is required");
        }

        var movieReviews = await _repository.ListAsync(new MovieReviewQueryCriteria(request.UserId));

        return new MovieReviewsListResponse(MovieReviewMapper.ToResponseList(movieReviews));
    }
}