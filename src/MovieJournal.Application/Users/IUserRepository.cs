using MovieJournal.Domain.Entities;

namespace MovieJournal.Application.Users;

public interface IUserRepository
{
    Task<User> CreateAsync(User user);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(Guid id);
    Task<bool> ExistsByEmailAsync(string email);
}
