using MovieJournal.Application.Exceptions;
using MovieJournal.Application.Security;
using MovieJournal.Application.Users.Requests;
using MovieJournal.Application.Users.Responses;

namespace MovieJournal.Application.Users.Commands;

public class LoginUserCmd
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public LoginUserCmd(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<UserResponse> Execute(LoginUserRequest request)
    {
        EnsureRequired(request.Email);
        EnsureRequired(request.Password);

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await _userRepository.GetByEmailAsync(normalizedEmail);

        if (user is null)
        {
            throw new UseCaseException("Invalid credentials");
        }

        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new UseCaseException("Invalid credentials");
        }

        var token = _tokenService.CreateToken(user);

        return new UserResponse(
            user.Id,
            user.DisplayName,
            user.Email,
            token);
    }

    private static void EnsureRequired(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new UseCaseException("Invalid credentials");
        }
    }
}
