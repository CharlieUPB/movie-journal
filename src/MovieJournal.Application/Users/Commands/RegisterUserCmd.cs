using MovieJournal.Application.Exceptions;
using MovieJournal.Application.Security;
using MovieJournal.Application.Users.Requests;
using MovieJournal.Application.Users.Responses;
using MovieJournal.Domain.Entities;

namespace MovieJournal.Application.Users.Commands;

public class RegisterUserCmd
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public RegisterUserCmd(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<UserResponse> Execute(RegisterUserRequest request)
    {
        EnsureRequired(request.DisplayName, "Display name is required");
        EnsureRequired(request.Email, "Email is required");
        EnsureRequired(request.Password, "Password is required");

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        if (await _userRepository.ExistsByEmailAsync(normalizedEmail))
        {
            throw new UseCaseException("Email is already registered");
        }

        var passwordHash = _passwordHasher.HashPassword(request.Password);
        var user = User.Create(request.DisplayName, normalizedEmail, passwordHash);

        var createdUser = await _userRepository.CreateAsync(user);
        var token = _tokenService.CreateToken(createdUser);

        return new UserResponse(
            createdUser.Id,
            createdUser.DisplayName,
            createdUser.Email,
            token);
    }

    private static void EnsureRequired(string? value, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new UseCaseException(message);
        }
    }
}
