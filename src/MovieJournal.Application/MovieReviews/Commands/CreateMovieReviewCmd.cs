using MovieJournal.Application.Common;
using MovieJournal.Application.MovieReviews.Requests;
using MovieJournal.Domain.Entities;
using MovieJournal.Domain.ValueObjects;

namespace MovieJournal.Application.MovieReviews.Commands;

public class CreateMovieReviewCmd : ICommand<CreateMovieRequest, MovieReview>
{
    private static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.Today);
    private readonly IMovieReviewsRepository _repository;

    public CreateMovieReviewCmd(IMovieReviewsRepository repository)
    {
        _repository = repository;
    }

    public async Task<MovieReview> Execute(CreateMovieRequest request)
    {
        var movieInformation = MovieInformation.Create(request.MovieTitle, Today, request.MovieReleaseYear);
        var reviewInformation = ReviewInformation.Create(request.ReviewTitle, request.ReviewContent, request.ReviewRating);
        var movieReview = MovieReview.Create(request.UserId, movieInformation, reviewInformation);

        return await _repository.CreateAsync(movieReview);
    }
}

