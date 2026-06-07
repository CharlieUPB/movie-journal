namespace MovieJournal.Application.Users.Responses;

public record UserResponse(
    Guid UserId,
    string DisplayName,
    string Email,
    string Token);
