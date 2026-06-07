namespace MovieJournal.Application.Users.Requests;

public record RegisterUserRequest(
    string DisplayName,
    string Email,
    string Password);
