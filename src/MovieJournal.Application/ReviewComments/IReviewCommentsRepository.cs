using MovieJournal.Domain.Entities;

namespace MovieJournal.Application.ReviewComments;

public interface IReviewCommentsRepository
{
    #region Command
    Task<ReviewComment> CreateAsync(ReviewComment reviewComment);
    Task<ReviewComment> UpdateAsync(ReviewComment reviewComment);
    Task<ReviewComment> DeleteAsync(ReviewComment reviewComment);
    #endregion

    #region Query
    Task<ReviewComment?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<ReviewComment>> GetByMovieReviewIdAsync(Guid movieReviewId);
    #endregion
}
