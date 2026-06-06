using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MovieJournal.Application.MovieReviews;
using MovieJournal.Infrastructure.persistence;
using MovieJournal.Infrastructure.Persistence.MovieReviews;

namespace MovieJournal.Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MovieJournalDb");

        services.AddScoped<ISqlConnectionFactory>(_ => new SqliteConnectionFactory(connectionString));

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IMovieReviewsRepository, MovieReviewsRepository>();

        return services;
    }
}
