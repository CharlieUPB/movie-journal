using MovieJournal.Web.ExceptionHandling;

namespace MovieJournal.Web;

public static class WebServiceExtensions
{

    public static IServiceCollection AddWebServices(this IServiceCollection services)
    {
        services.AddExceptionHandler<DomainExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }
}
