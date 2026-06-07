using MovieJournal.Domain.Entities;

namespace MovieJournal.Application.MovieReviews;

public interface IMovieReviewsRepository
{
    #region Command
    Task<MovieReview> CreateAsync(MovieReview movieReview);
    Task<MovieReview> UpdateAsync(MovieReview movieReview);
    Task<MovieReview> DeleteAsync(MovieReview movieReview);
    #endregion

    #region Query
    Task<MovieReview?> GetByIdAsync(Guid id);

    Task<IReadOnlyList<MovieReview>> ListAsync(MovieReviewQueryCriteria criteria);
    #endregion
}
