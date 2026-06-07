namespace MovieJournal.Web.Controllers.Users;

public record RegisterUserHttpRequest(
    string DisplayName,
    string Email,
    string Password);
