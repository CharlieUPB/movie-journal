using MovieJournal.Domain.Enums;

namespace MovieJournal.Application.MovieReviews;

public sealed record MovieReviewQueryCriteria(Guid? UserId = null, ReviewStatus? Status = null, bool IncludeDeleted = false);