namespace MovieJournal.Application.Users.Requests;

public record LoginUserRequest(
    string Email,
    string Password);
