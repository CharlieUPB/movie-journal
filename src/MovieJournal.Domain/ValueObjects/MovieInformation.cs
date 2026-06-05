using MovieJournal.Domain.Exceptions;

namespace MovieJournal.Domain.ValueObjects
{
    public record MovieInformation
    {
        public string MovieTitle { get; init; }

        public int? ReleaseYear { get; init; }

        private MovieInformation(string? movieTitle, DateOnly today, int? releaseYear = null)
        {
            if (string.IsNullOrWhiteSpace(movieTitle))
            {
                throw new DomainException("Movie title is required");
            }

            if (releaseYear.HasValue && releaseYear.Value > today.Year)
            {
                throw new DomainException("Release year cannot be in the future.");
            }

            MovieTitle = movieTitle.Trim();
            ReleaseYear = releaseYear;
        }

        public static MovieInformation Create(string? movieTitle, DateOnly today, int? releaseYear = null)
        {
            return new MovieInformation(movieTitle, today, releaseYear);
        }
    }
}
