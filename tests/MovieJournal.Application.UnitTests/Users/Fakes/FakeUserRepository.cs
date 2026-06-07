using MovieJournal.Application.Users;
using MovieJournal.Domain.Entities;

namespace MovieJournal.Application.UnitTests.Users.Fakes;

internal class FakeUserRepository : IUserRepository
{
    public List<User> Users { get; } = new();
    public int CreateAsyncCallCount { get; private set; }

    public Task<User> CreateAsync(User user)
    {
        CreateAsyncCallCount++;
        Users.Add(user);

        return Task.FromResult(user);
    }

    public Task<User?> GetByEmailAsync(string email)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        return Task.FromResult(
            Users.FirstOrDefault(user => user.Email == normalizedEmail && user.IsDeleted != true));
    }

    public Task<User?> GetByIdAsync(Guid id)
    {
        return Task.FromResult(
            Users.FirstOrDefault(user => user.Id == id && user.IsDeleted != true));
    }

    public Task<bool> ExistsByEmailAsync(string email)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        return Task.FromResult(
            Users.Any(user => user.Email == normalizedEmail && user.IsDeleted != true));
    }
}
