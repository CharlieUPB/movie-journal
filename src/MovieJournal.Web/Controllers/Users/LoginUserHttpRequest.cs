namespace MovieJournal.Web.Controllers.Users;

public record LoginUserHttpRequest(
    string Email,
    string Password);
