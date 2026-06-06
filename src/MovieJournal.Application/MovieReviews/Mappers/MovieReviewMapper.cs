using MovieJournal.Application.MovieReviews.Responses;
using MovieJournal.Domain.Entities;

namespace MovieJournal.Application.MovieReviews.Mappers
{
    public static class MovieReviewMapper
    {
        public static MovieReviewResponse ToResponse(MovieReview movieReview)
        {
            return new MovieReviewResponse(
                movieReview.Id,
                movieReview.UserId,
                movieReview.MovieInformation.MovieTitle,
                movieReview.MovieInformation.ReleaseYear,
                movieReview.ReviewInformation.ReviewTitle,
                movieReview.ReviewInformation.ReviewContent,
                movieReview.ReviewInformation.Rating,
                movieReview.Status.ToString());
        }
    }
}
