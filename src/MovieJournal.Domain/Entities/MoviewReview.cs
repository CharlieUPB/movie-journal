namespace MovieJournal.Domain.Entities;

using MovieJournal.Domain.Common;
using MovieJournal.Domain.Enums;
using MovieJournal.Domain.Exceptions;
using MovieJournal.Domain.ValueObjects;

public class MovieReview : AuditableEntity
{
	private readonly List<ReviewComment> _comments = new();
	public Guid UserId { get; private set; }

	public MovieInformation MovieInformation { get; private set; }

	public ReviewInformation ReviewInformation { get; private set; }

	public ReviewStatus Status { get; private set; }

	public IReadOnlyCollection<ReviewComment> Comments => _comments.AsReadOnly();

	private MovieReview(Guid userId, MovieInformation movieInformation, ReviewInformation reviewInformation)
	{
		UserId = userId;
		MovieInformation = movieInformation;
		ReviewInformation = reviewInformation;
		Status = ReviewStatus.Draft;
	}

	public static MovieReview Create(Guid userId, MovieInformation movieInformation, ReviewInformation reviewInformation)
	{
		return new MovieReview(userId, movieInformation, reviewInformation);
	}

	public void UpdateMovieReview(MovieInformation movieInformation,ReviewInformation reviewInformation)
	{
		if (Status == ReviewStatus.Archived)
		{
			throw new DomainException("Archived reviews cannot be updated");
		}

		MovieInformation = movieInformation;
		ReviewInformation = reviewInformation;

		MarkAsUpdated();
	}

	public void PublishReview()
	{
		if (Status != ReviewStatus.Draft)
		{
			throw new DomainException("Only draft reviews can be published");
		}

		Status = ReviewStatus.Published;

		MarkAsUpdated();
    }

	public void ArchiveReview()
	{
		if (Status == ReviewStatus.Archived)
		{
			return;
		}

		Status = ReviewStatus.Archived;

		MarkAsUpdated();
	}

	public void Delete()
	{
		MarkAsUpdated();
		MarkAsDeleted();
	}

	public void AddComment(ReviewComment comment)
	{
        if (Status == ReviewStatus.Archived)
        {
            throw new DomainException("Can't add comments to archived reviews");
        }

        _comments.Add(comment);

		MarkAsUpdated();
	}
}
