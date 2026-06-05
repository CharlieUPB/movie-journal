using MovieJournal.Domain.Common;
using MovieJournal.Domain.Exceptions;

namespace MovieJournal.Domain.Entities;

public class ReviewComment : AuditableEntity
{
    private const int MaximumCommentLength = 500;

    public Guid MoviewReviewId { get; private set; }
    public Guid UserId { get; private set; }

    public string Content { get; private set; }

    private ReviewComment(Guid moviewReviewId, Guid userId, string? content)
    {

        if (string.IsNullOrWhiteSpace(content) || content.Length > MaximumCommentLength)
        {
            throw new DomainException($"Comment content is requiered and should not exceed {MaximumCommentLength} chars");
        }

        MoviewReviewId = moviewReviewId;
        UserId = userId;
        Content = content;
    }

    public static ReviewComment Create(Guid movieReviewId, Guid userId, string? content)
    {
        return new ReviewComment(movieReviewId, userId, content);
    }
}