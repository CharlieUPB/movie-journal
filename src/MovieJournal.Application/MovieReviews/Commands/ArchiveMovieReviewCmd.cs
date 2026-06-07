using MovieJournal.Application.Common;
using MovieJournal.Application.Exceptions;
using MovieJournal.Application.MovieReviews.Mappers;
using MovieJournal.Application.MovieReviews.Requests;
using MovieJournal.Application.MovieReviews.Responses;

namespace MovieJournal.Application.MovieReviews.Commands;

public class ArchiveMovieReviewCmd : ICommand<ArchiveMovieReviewRequest, MovieReviewResponse>
{
    private readonly IMovieReviewsRepository _repository;

    public ArchiveMovieReviewCmd(IMovieReviewsRepository repository)
    {
        _repository = repository;
    }

    public async Task<MovieReviewResponse> Execute(ArchiveMovieReviewRequest request)
    {
        var movieReview = await _repository.GetByIdAsync(request.MovieReviewId);

        if (movieReview is null)
        {
            throw new UseCaseException("Movie review was not found");
        }

        if (movieReview.UserId != request.UserId)
        {
            throw new UseCaseException("You are not allowed to archive this movie review");
        }

        movieReview.ArchiveReview();

        var updatedMovieReview = await _repository.UpdateAsync(movieReview);

        return MovieReviewMapper.ToResponse(updatedMovieReview);
    }
}
