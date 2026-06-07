using System.Text.Json.Serialization;
using TaksManagementAI.API.Validator;
using DomainTask = TaksManagementAI.API.Domain.Task;

namespace TaksManagementAI.API.Application;

public sealed record TaskResponse(
    Guid Id,
    Guid UserId,
    string Title,
    string Description,
    string Status,
    [property: JsonPropertyName("due_date")] DateOnly? DueDate,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt)
{
    public static TaskResponse FromDomain(DomainTask task)
        => new(
            task.Id,
            task.UserId,
            task.Title,
            task.Description,
            TaskValidator.FormatStatus(task.Status),
            task.DueDate,
            task.CreatedAt,
            task.UpdatedAt);
}
