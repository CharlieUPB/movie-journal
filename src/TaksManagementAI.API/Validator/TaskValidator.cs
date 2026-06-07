using DomainResult = TaksManagementAI.API.Domain.Result;
using DomainTaskStatus = TaksManagementAI.API.Domain.TaskStatus;

namespace TaksManagementAI.API.Validator;

public static class TaskValidator
{
    public static DomainResult ValidateCreate(Guid userId, string? title, string? description)
    {
        if (userId == Guid.Empty)
        {
            return DomainResult.Failure("userId is required.");
        }

        return ValidateRequiredFields(title, description);
    }

    public static DomainResult ValidateRequiredFields(string? title, string? description)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return DomainResult.Failure("title is required.");
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            return DomainResult.Failure("description is required.");
        }

        return DomainResult.Success();
    }

    public static TaksManagementAI.API.Domain.Result<DomainTaskStatus> ParseStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            return TaksManagementAI.API.Domain.Result<DomainTaskStatus>.Failure("status is required.");
        }

        return status.Trim().ToUpperInvariant() switch
        {
            "TODO" => TaksManagementAI.API.Domain.Result<DomainTaskStatus>.Success(DomainTaskStatus.Todo),
            "IN PROGRESS" or "IN_PROGRESS" or "INPROGRESS" => TaksManagementAI.API.Domain.Result<DomainTaskStatus>.Success(DomainTaskStatus.InProgress),
            "DONE" => TaksManagementAI.API.Domain.Result<DomainTaskStatus>.Success(DomainTaskStatus.Done),
            _ => TaksManagementAI.API.Domain.Result<DomainTaskStatus>.Failure("status must be one of: TODO, In Progress, Done.")
        };
    }

    public static string FormatStatus(DomainTaskStatus status)
        => status switch
        {
            DomainTaskStatus.Todo => "TODO",
            DomainTaskStatus.InProgress => "In Progress",
            DomainTaskStatus.Done => "Done",
            _ => status.ToString()
        };

    public static string NormalizeRequiredText(string? value) => value?.Trim() ?? string.Empty;
}
