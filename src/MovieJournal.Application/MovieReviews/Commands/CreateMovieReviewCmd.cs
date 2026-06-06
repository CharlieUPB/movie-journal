using MovieJournal.Application.Common;
using MovieJournal.Application.MovieReviews.Mappers;
using MovieJournal.Application.MovieReviews.Requests;
using MovieJournal.Application.MovieReviews.Responses;
using MovieJournal.Domain.Entities;
using MovieJournal.Domain.ValueObjects;

namespace MovieJournal.Application.MovieReviews.Commands;

public class CreateMovieReviewCmd : ICommand<CreateMovieRequest, MovieReviewResponse>
{
    private static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.Today);
    private readonly IMovieReviewsRepository _repository;

    public CreateMovieReviewCmd(IMovieReviewsRepository repository)
    {
        _repository = repository;
    }

    public async Task<MovieReviewResponse> Execute(CreateMovieRequest request)
    {
        var movieInformation = MovieInformation.Create(request.MovieTitle, Today, request.MovieReleaseYear);
        var reviewInformation = ReviewInformation.Create(request.ReviewTitle, request.ReviewContent, request.ReviewRating);
        var movieReview = MovieReview.Create(request.UserId, movieInformation, reviewInformation);

        var createdMovieReview = await _repository.CreateAsync(movieReview);

        return MovieReviewMapper.ToResponse(createdMovieReview);
    }
}

