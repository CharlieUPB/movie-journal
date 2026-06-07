using MovieJournal.Application.Common;
using MovieJournal.Application.Exceptions;
using MovieJournal.Application.MovieReviews;
using MovieJournal.Application.ReviewComments.Requests;

namespace MovieJournal.Application.ReviewComments.Commands;

public class DeleteReviewCommentCmd : ICommand<DeleteReviewCommentRequest>
{
    private readonly IMovieReviewsRepository _movieReviewsRepository;
    private readonly IReviewCommentsRepository _reviewCommentsRepository;

    public DeleteReviewCommentCmd(
        IMovieReviewsRepository movieReviewsRepository,
        IReviewCommentsRepository reviewCommentsRepository)
    {
        _movieReviewsRepository = movieReviewsRepository;
        _reviewCommentsRepository = reviewCommentsRepository;
    }

    public async Task Execute(DeleteReviewCommentRequest request)
    {
        var reviewComment = await _reviewCommentsRepository.GetByIdAsync(request.CommentId);

        if (reviewComment is null || reviewComment.IsDeleted == true)
        {
            return;
        }

        if (reviewComment.UserId != request.UserId)
        {
            throw new UseCaseException("You are not allowed to delete this review comment");
        }

        var movieReview = await _movieReviewsRepository.GetByIdAsync(reviewComment.MovieReviewId);

        if (movieReview is null)
        {
            throw new UseCaseException("Movie review was not found");
        }

        movieReview.EnsureCanReceiveComments();

        reviewComment.Delete();

        await _reviewCommentsRepository.DeleteAsync(reviewComment);
    }
}
