using Microsoft.Extensions.DependencyInjection;
using MovieJournal.Application.MovieReviews.Commands;
using MovieJournal.Application.MovieReviews.Queries;

namespace MovieJournal.Application;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<CreateMovieReviewCmd>();
        services.AddScoped<UpdateMovieReviewCmd>();
        services.AddScoped<DeleteMovieReviewCmd>();

        services.AddScoped<GetMovieReviewQuery>();
        services.AddScoped<ListMovieReviewsByUserIdAndStatusQuery>();
        services.AddScoped<ListMovieReviewsByUserIdQuery>();
        services.AddScoped<ListMovieReviewsQuery>();
        services.AddScoped<ListPublishedMovieReviewsQuery>();

        return services;
    }
}
