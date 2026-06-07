using System.Security.Claims;

namespace MovieJournal.Web.Authentication;

public static class CurrentUserExtensions
{
    public static Guid GetRequiredUserId(this ClaimsPrincipal user)
    {
        var userId = user.GetOptionalUserId();

        if (!userId.HasValue)
        {
            throw new UnauthorizedAccessException("Authenticated user id claim is missing or invalid.");
        }

        return userId.Value;
    }

    public static Guid? GetOptionalUserId(this ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return Guid.TryParse(userIdClaim, out var userId)
            ? userId
            : null;
    }
}
