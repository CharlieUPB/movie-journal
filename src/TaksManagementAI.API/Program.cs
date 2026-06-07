using TaksManagementAI.API.Application;
using TaksManagementAI.API.Controller;
using TaksManagementAI.API.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<CreateTaskCmd>();
builder.Services.AddScoped<UpdateTaskCmd>();
builder.Services.AddScoped<UpdateTaskStatusCmd>();
builder.Services.AddScoped<DeleteTaskCmd>();
builder.Services.AddScoped<GetTaskQuery>();
builder.Services.AddScoped<ListTasksQuery>();

var app = builder.Build();

app.MapGet("/", () => Results.Redirect("/tasks"));
app.MapTaskEndpoints();

app.Run();

public partial class Program
{
}
