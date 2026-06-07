using MovieJournal.Application.Common;
using MovieJournal.Application.Exceptions;
using MovieJournal.Application.MovieReviews.Requests;

namespace MovieJournal.Application.MovieReviews.Commands;

public class DeleteMovieReviewCmd : ICommand<DeleteMovieReviewRequest>
{
    private readonly IMovieReviewsRepository _repository;

    public DeleteMovieReviewCmd(IMovieReviewsRepository repository)
    {
        _repository = repository;
    }

    public async Task Execute(DeleteMovieReviewRequest request)
    {
        var movieReview = await _repository.GetByIdAsync(request.MovieReviewId);

        if (movieReview is null)
        {
            return;
        }

        if (movieReview.UserId != request.UserId)
        {
            throw new UseCaseException("You are not allowed to delete this movie review");
        }

        movieReview.Delete();

        await _repository.DeleteAsync(movieReview);
    }
}
