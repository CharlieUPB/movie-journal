using MovieJournal.Application.Common;
using MovieJournal.Application.MovieReviews.Mappers;
using MovieJournal.Application.MovieReviews.Requests;
using MovieJournal.Application.MovieReviews.Responses;

namespace MovieJournal.Application.MovieReviews.Queries;

public class ListMovieReviewsQuery : IQuery<ListMovieReviewsRequest, MovieReviewsListResponse>
{
    private readonly IMovieReviewsRepository _repository;

    public ListMovieReviewsQuery(IMovieReviewsRepository repository)
    {
        _repository = repository;
    }

    public async Task<MovieReviewsListResponse> Execute(ListMovieReviewsRequest request)
    {
        var reviews = await _repository.ListAsync(new MovieReviewQueryCriteria());

        return new MovieReviewsListResponse(MovieReviewMapper.ToResponseList(reviews));
    }
}