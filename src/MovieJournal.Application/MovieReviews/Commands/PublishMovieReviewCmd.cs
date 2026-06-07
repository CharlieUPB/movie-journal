using MovieJournal.Application.Common;
using MovieJournal.Application.Exceptions;
using MovieJournal.Application.MovieReviews.Mappers;
using MovieJournal.Application.MovieReviews.Requests;
using MovieJournal.Application.MovieReviews.Responses;

namespace MovieJournal.Application.MovieReviews.Commands;

public class PublishMovieReviewCmd : ICommand<PublishMovieReviewRequest, MovieReviewResponse>
{
    private readonly IMovieReviewsRepository _repository;

    public PublishMovieReviewCmd(IMovieReviewsRepository repository)
    {
        _repository = repository;
    }

    public async Task<MovieReviewResponse> Execute(PublishMovieReviewRequest request)
    {
        var movieReview = await _repository.GetByIdAsync(request.MovieReviewId);

        if (movieReview is null)
        {
            throw new UseCaseException("Movie review was not found");
        }

        if (movieReview.UserId != request.UserId)
        {
            throw new UseCaseException("You are not allowed to publish this movie review");
        }

        movieReview.PublishReview();

        var updatedMovieReview = await _repository.UpdateAsync(movieReview);

        return MovieReviewMapper.ToResponse(updatedMovieReview);
    }
}
