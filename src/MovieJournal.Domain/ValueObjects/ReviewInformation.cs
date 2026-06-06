using MovieJournal.Domain.Exceptions;

namespace MovieJournal.Domain.ValueObjects
{
    public record ReviewInformation
    {
        private const int MinimumReviewContentLenght = 50;

        public string ReviewTitle { get; init; }

        public string ReviewContent { get; init; }

        public int Rating { get; init; }

        private ReviewInformation(string? title, string? content, int? rating)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new DomainException("Review title is required");
            }

            if (string.IsNullOrWhiteSpace(content) || content.Length < MinimumReviewContentLenght)
            {
                throw new DomainException($"Content is required and must be minimum of {MinimumReviewContentLenght} characters");
            }

            if (!rating.HasValue || rating < 1 || rating > 5)
            {
                throw new DomainException($"Rating {rating} must be between 1 and 5");
            }

            ReviewTitle = title.Trim();
            ReviewContent = content.Trim();
            Rating = rating.Value;
        }

        public static ReviewInformation Create(string? title, string? content, int? rating)
        {
            return new ReviewInformation(title, content, rating);
        }
    }
}
