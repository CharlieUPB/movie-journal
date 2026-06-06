using MovieJournal.Web;
using MovieJournal.Application;
using MovieJournal.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplicationServices()
    .AddRepositories()
    .AddPersistence(builder.Configuration)
    .AddWebServices()
    .AddControllers();

builder.Services.AddOpenApi();

var app = builder.Build();

await app.InitializeDatabaseAsync();

app.UseExceptionHandler(options => { });

app.MapOpenApi();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI(options =>
    {
        // Map Swagger UI to point to the native .NET 10 OpenAPI JSON schema route
        options.SwaggerEndpoint("/openapi/v1.json", "My API v1");
    });
}

app.MapControllers();

app.Run();
