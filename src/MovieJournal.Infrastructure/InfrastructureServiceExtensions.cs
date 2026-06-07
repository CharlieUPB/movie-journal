using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MovieJournal.Application.MovieReviews;
using MovieJournal.Application.ReviewComments;
using MovieJournal.Application.Security;
using MovieJournal.Application.Users;
using MovieJournal.Infrastructure.Persistence.Connection;
using MovieJournal.Infrastructure.Persistence.Initializer;
using MovieJournal.Infrastructure.Persistence.MovieReviews;
using MovieJournal.Infrastructure.Persistence.ReviewComments;
using MovieJournal.Infrastructure.Persistence.Users;
using MovieJournal.Infrastructure.Security;

namespace MovieJournal.Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MovieJournalDb");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'MovieJournalDb' was not found.");
        }

        services.AddScoped<ISqlConnectionFactory>(_ => new SqliteConnectionFactory(connectionString));
        services.AddScoped<IDatabaseInitializer, SqliteDatabaseInitializer>();

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IMovieReviewsRepository, MovieReviewsRepository>();
        services.AddScoped<IReviewCommentsRepository, ReviewCommentsRepository>();

        return services;
    }

    public static IServiceCollection AddInfrastructureSecurity(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, JwtTokenService>();

        return services;
    }

}
