using MovieJournal.Application.Exceptions;
using MovieJournal.Application.UnitTests.Users.Fakes;
using MovieJournal.Application.Users.Commands;
using MovieJournal.Application.Users.Requests;
using MovieJournal.Domain.Entities;

namespace MovieJournal.Application.UnitTests.Users;

public class UserCommandsTests
{
    [Fact]
    public async Task RegisterUserCmd_ShouldCreateUserAndReturnToken()
    {
        var userRepository = new FakeUserRepository();
        var command = new RegisterUserCmd(
            userRepository,
            new FakePasswordHasher(),
            new FakeTokenService());
        var request = new RegisterUserRequest("Demo User", "DEMO@MovieJournal.COM", "Demo123!");

        var response = await command.Execute(request);

        Assert.Equal(1, userRepository.CreateAsyncCallCount);
        Assert.NotEqual(Guid.Empty, response.UserId);
        Assert.Equal("Demo User", response.DisplayName);
        Assert.Equal("demo@moviejournal.com", response.Email);
        Assert.Equal($"token:{response.UserId}", response.Token);
        Assert.Equal("hashed:Demo123!", userRepository.Users.Single().PasswordHash);
    }

    [Fact]
    public async Task RegisterUserCmd_ShouldThrowUseCaseExceptionWhenEmailAlreadyExists()
    {
        var userRepository = new FakeUserRepository();
        userRepository.Users.Add(User.Create("Demo User", "demo@moviejournal.com", "hashed:Demo123!"));
        var command = new RegisterUserCmd(
            userRepository,
            new FakePasswordHasher(),
            new FakeTokenService());
        var request = new RegisterUserRequest("Other User", "DEMO@MovieJournal.COM", "Demo123!");

        var exception = await Assert.ThrowsAsync<UseCaseException>(() => command.Execute(request));

        Assert.Equal("Email is already registered", exception.Message);
        Assert.Equal(0, userRepository.CreateAsyncCallCount);
    }

    [Fact]
    public async Task LoginUserCmd_ShouldReturnTokenForValidCredentials()
    {
        var userRepository = new FakeUserRepository();
        var user = User.Create("Demo User", "demo@moviejournal.com", "hashed:Demo123!");
        userRepository.Users.Add(user);
        var command = new LoginUserCmd(
            userRepository,
            new FakePasswordHasher(),
            new FakeTokenService());
        var request = new LoginUserRequest("DEMO@MovieJournal.COM", "Demo123!");

        var response = await command.Execute(request);

        Assert.Equal(user.Id, response.UserId);
        Assert.Equal("Demo User", response.DisplayName);
        Assert.Equal("demo@moviejournal.com", response.Email);
        Assert.Equal($"token:{user.Id}", response.Token);
    }

    [Fact]
    public async Task LoginUserCmd_ShouldThrowUseCaseExceptionForInvalidCredentials()
    {
        var userRepository = new FakeUserRepository();
        userRepository.Users.Add(User.Create("Demo User", "demo@moviejournal.com", "hashed:Demo123!"));
        var command = new LoginUserCmd(
            userRepository,
            new FakePasswordHasher(),
            new FakeTokenService());
        var request = new LoginUserRequest("demo@moviejournal.com", "WrongPassword");

        var exception = await Assert.ThrowsAsync<UseCaseException>(() => command.Execute(request));

        Assert.Equal("Invalid credentials", exception.Message);
    }
}
