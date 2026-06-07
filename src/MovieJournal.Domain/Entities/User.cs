using MovieJournal.Domain.Common;
using MovieJournal.Domain.Exceptions;

namespace MovieJournal.Domain.Entities;

public class User : AuditableEntity
{
    public string DisplayName { get; private set; }

    public string Email { get; private set; }

    public string PasswordHash { get; private set; }

    private User(
        Guid id,
        string displayName,
        string email,
        string passwordHash,
        DateTime createdAt,
        DateTime? updatedAt,
        bool? isDeleted)
    {
        Id = id;
        DisplayName = displayName;
        Email = NormalizeEmail(email);
        PasswordHash = passwordHash;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        IsDeleted = isDeleted;
    }

    private User(string? displayName, string? email, string? passwordHash)
    {
        DisplayName = ValidateDisplayName(displayName);
        Email = ValidateEmail(email);
        PasswordHash = ValidatePasswordHash(passwordHash);
    }

    public static User Create(string? displayName, string? email, string? passwordHash)
    {
        return new User(displayName, email, passwordHash);
    }

    public static User Rebuild(
        Guid id,
        string displayName,
        string email,
        string passwordHash,
        DateTime createdAt,
        DateTime? updatedAt,
        bool? isDeleted)
    {
        return new User(
            id,
            displayName,
            email,
            passwordHash,
            createdAt,
            updatedAt,
            isDeleted);
    }

    private static string ValidateDisplayName(string? displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new DomainException("Display name is required");
        }

        return displayName.Trim();
    }

    private static string ValidateEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new DomainException("Email is required");
        }

        return NormalizeEmail(email);
    }

    private static string ValidatePasswordHash(string? passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new DomainException("Password hash is required");
        }

        return passwordHash;
    }

    private static string NormalizeEmail(string email)
    {
        return email.Trim().ToLowerInvariant();
    }
}
