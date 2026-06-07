using System.Text.Json.Serialization;
using TaksManagementAI.API.Application;
using ErrorType = TaksManagementAI.API.Domain.ErrorType;

namespace TaksManagementAI.API.Controller;

public static class TaskController
{
    public static IEndpointRouteBuilder MapTaskEndpoints(this IEndpointRouteBuilder app)
    {
        var tasks = app.MapGroup("/tasks").WithTags("Tasks");

        tasks.MapGet("", List);
        tasks.MapGet("/{id:guid}", GetById);
        tasks.MapPost("", Create);
        tasks.MapPut("/{id:guid}", Update);
        tasks.MapPatch("/{id:guid}/status", UpdateStatus);
        tasks.MapDelete("/{id:guid}", Delete);

        return app;
    }

    private static async Task<IResult> List(
        Guid? userId,
        ListTasksQuery query,
        CancellationToken cancellationToken)
    {
        var result = await query.Handle(new ListTasksRequest(userId), cancellationToken);

        return result.IsSuccess ? Results.Ok(result.Value) : ToError(result);
    }

    private static async Task<IResult> GetById(
        Guid id,
        GetTaskQuery query,
        CancellationToken cancellationToken)
    {
        var result = await query.Handle(new GetTaskRequest(id), cancellationToken);

        return result.IsSuccess ? Results.Ok(result.Value) : ToError(result);
    }

    private static async Task<IResult> Create(
        CreateTaskHttpRequest request,
        CreateTaskCmd command,
        CancellationToken cancellationToken)
    {
        var result = await command.Handle(
            new CreateTaskRequest(request.UserId, request.Title, request.Description, request.DueDate),
            cancellationToken);

        return result.IsSuccess
            ? Results.Created($"/tasks/{result.Value!.Id}", result.Value)
            : ToError(result);
    }

    private static async Task<IResult> Update(
        Guid id,
        UpdateTaskHttpRequest request,
        UpdateTaskCmd command,
        CancellationToken cancellationToken)
    {
        var result = await command.Handle(
            id,
            new UpdateTaskRequest(request.UserId, request.Title, request.Description, request.Status, request.DueDate),
            cancellationToken);

        return result.IsSuccess ? Results.Ok(result.Value) : ToError(result);
    }

    private static async Task<IResult> UpdateStatus(
        Guid id,
        UpdateTaskStatusHttpRequest request,
        UpdateTaskStatusCmd command,
        CancellationToken cancellationToken)
    {
        var result = await command.Handle(
            id,
            new UpdateTaskStatusRequest(request.UserId, request.Status),
            cancellationToken);

        return result.IsSuccess ? Results.Ok(result.Value) : ToError(result);
    }

    private static async Task<IResult> Delete(
        Guid id,
        Guid userId,
        DeleteTaskCmd command,
        CancellationToken cancellationToken)
    {
        var result = await command.Handle(id, new DeleteTaskRequest(userId), cancellationToken);

        return result.IsSuccess ? Results.NoContent() : ToError(result);
    }

    private static IResult ToError<T>(TaksManagementAI.API.Domain.Result<T> result)
        => ToError(result.Error ?? "Operation failed.", result.ErrorType);

    private static IResult ToError(TaksManagementAI.API.Domain.Result result)
        => ToError(result.Error ?? "Operation failed.", result.ErrorType);

    private static IResult ToError(string error, ErrorType errorType)
    {
        var response = new ErrorResponse(error);

        return errorType switch
        {
            ErrorType.NotFound => Results.NotFound(response),
            ErrorType.Forbidden => Results.Json(response, statusCode: StatusCodes.Status403Forbidden),
            ErrorType.Conflict => Results.Conflict(response),
            _ => Results.BadRequest(response)
        };
    }

    public sealed record CreateTaskHttpRequest(
        Guid UserId,
        string? Title,
        string? Description,
        [property: JsonPropertyName("due_date")] DateOnly? DueDate);

    public sealed record UpdateTaskHttpRequest(
        Guid UserId,
        string? Title,
        string? Description,
        string? Status,
        [property: JsonPropertyName("due_date")] DateOnly? DueDate);

    public sealed record UpdateTaskStatusHttpRequest(Guid UserId, string? Status);

    private sealed record ErrorResponse(string Error);
}
