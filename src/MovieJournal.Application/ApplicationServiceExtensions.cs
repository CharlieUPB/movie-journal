using Microsoft.Extensions.DependencyInjection;
using MovieJournal.Application.MovieReviews.Commands;
using MovieJournal.Application.MovieReviews.Queries;
using MovieJournal.Application.ReviewComments.Commands;
using MovieJournal.Application.ReviewComments.Queries;

namespace MovieJournal.Application;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<CreateMovieReviewCmd>();
        services.AddScoped<UpdateMovieReviewCmd>();
        services.AddScoped<DeleteMovieReviewCmd>();
        services.AddScoped<PublishMovieReviewCmd>();
        services.AddScoped<ArchiveMovieReviewCmd>();

        services.AddScoped<AddReviewCommentCmd>();
        services.AddScoped<UpdateReviewCommentCmd>();
        services.AddScoped<DeleteReviewCommentCmd>();

        services.AddScoped<GetMovieReviewQuery>();
        services.AddScoped<ListMovieReviewsByUserIdAndStatusQuery>();
        services.AddScoped<ListMovieReviewsByUserIdQuery>();
        services.AddScoped<ListMovieReviewsQuery>();
        services.AddScoped<ListPublishedMovieReviewsQuery>();
        
        services.AddScoped<ListReviewCommentsQuery>();

        return services;
    }
}
