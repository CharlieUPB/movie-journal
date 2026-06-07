using MovieJournal.Domain.Common;
using MovieJournal.Domain.Exceptions;

namespace MovieJournal.Domain.Entities;

public class ReviewComment : AuditableEntity
{
    private const int MaximumCommentLength = 500;

    public Guid MovieReviewId { get; private set; }
    public Guid UserId { get; private set; }

    public string Content { get; private set; }

    private ReviewComment(
        Guid id,
        Guid movieReviewId,
        Guid userId,
        string content,
        DateTime createdAt,
        DateTime? updatedAt,
        bool? isDeleted)
    {
        Id = id;
        MovieReviewId = movieReviewId;
        UserId = userId;
        Content = content;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        IsDeleted = isDeleted;
    }

    private ReviewComment(Guid movieReviewId, Guid userId, string? content)
    {
        MovieReviewId = movieReviewId;
        UserId = userId;
        Content = ValidateContent(content);
    }

    public static ReviewComment Create(Guid movieReviewId, Guid userId, string? content)
    {
        return new ReviewComment(movieReviewId, userId, content);
    }

    public static ReviewComment Rebuild(
        Guid id,
        Guid movieReviewId,
        Guid userId,
        string content,
        DateTime createdAt,
        DateTime? updatedAt,
        bool? isDeleted)
    {
        return new ReviewComment(
            id,
            movieReviewId,
            userId,
            content,
            createdAt,
            updatedAt,
            isDeleted);
    }

    public void UpdateComment(string? content)
    {
        Content = ValidateContent(content);

        MarkAsUpdated();
    }

    public void Delete()
    {
        if (IsDeleted == true)
        {
            return;
        }

        MarkAsUpdated();
        MarkAsDeleted();
    }

    private static string ValidateContent(string? content)
    {
        if (string.IsNullOrWhiteSpace(content) || content.Length > MaximumCommentLength)
        {
            throw new DomainException($"Comment content is requiered and should not exceed {MaximumCommentLength} chars");
        }

        return content.Trim();
    }
}
