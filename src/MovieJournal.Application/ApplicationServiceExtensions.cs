using Microsoft.Extensions.DependencyInjection;
using MovieJournal.Application.MovieReviews.Commands;

namespace MovieJournal.Application;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<CreateMovieReviewCmd>();

        return services;
    }
}
