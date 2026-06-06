using MovieJournal.Application.Common;
using MovieJournal.Application.Exceptions;
using MovieJournal.Application.MovieReviews.Mappers;
using MovieJournal.Application.MovieReviews.Requests;
using MovieJournal.Application.MovieReviews.Responses;
using MovieJournal.Domain.ValueObjects;

namespace MovieJournal.Application.MovieReviews.Commands;

public class UpdateMovieReviewCmd : ICommand<UpdateMovieReviewRequest, MovieReviewResponse>
{
    private static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.Today);
    private readonly IMovieReviewsRepository _repository;

    public UpdateMovieReviewCmd(IMovieReviewsRepository repository)
    {
        _repository = repository;
    }

    public async Task<MovieReviewResponse> Execute(UpdateMovieReviewRequest request)
    {
        var movieReview = await _repository.GetByIdAsync(request.MovieReviewId);

        if (movieReview is null)
        {
            throw new UseCaseException("Movie review was not found");
        }

        if (movieReview.UserId != request.UserId)
        {
            throw new UseCaseException("You are not allowed to update this movie review");
        }

        var movieInformation = MovieInformation.Create(
            request.MovieTitle,
            Today,
            request.MovieReleaseYear);

        var reviewInformation = ReviewInformation.Create(
            request.ReviewTitle,
            request.ReviewContent,
            request.ReviewRating);

        movieReview.UpdateMovieReview(movieInformation, reviewInformation);

        var updatedMovieReview = await _repository.UpdateAsync(movieReview);

        return MovieReviewMapper.ToResponse(updatedMovieReview);
    }
}
