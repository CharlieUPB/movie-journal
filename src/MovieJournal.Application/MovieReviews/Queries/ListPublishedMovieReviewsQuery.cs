using MovieJournal.Application.Common;
using MovieJournal.Application.MovieReviews.Mappers;
using MovieJournal.Application.MovieReviews.Requests;
using MovieJournal.Application.MovieReviews.Responses;
using MovieJournal.Domain.Enums;

namespace MovieJournal.Application.MovieReviews.Queries;

public class ListPublishedMovieReviewsQuery : IQuery<ListPublishedMovieReviewsRequest, MovieReviewsListResponse>
{
    private readonly IMovieReviewsRepository _repository;

    public ListPublishedMovieReviewsQuery(IMovieReviewsRepository repository)
    {
        _repository = repository;
    }

    public async Task<MovieReviewsListResponse> Execute(ListPublishedMovieReviewsRequest request)
    {
        var reviews = await _repository.ListAsync(new MovieReviewQueryCriteria(Status: ReviewStatus.Published));

        return new MovieReviewsListResponse(MovieReviewMapper.ToResponseList(reviews));
    }
}