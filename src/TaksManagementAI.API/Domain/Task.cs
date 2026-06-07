using TaksManagementAI.API.Validator;

namespace TaksManagementAI.API.Domain;

public sealed class Task
{
    private Task(
        Guid id,
        Guid userId,
        string title,
        string description,
        TaskStatus status,
        DateOnly? dueDate,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt)
    {
        Id = id;
        UserId = userId;
        Title = title;
        Description = description;
        Status = status;
        DueDate = dueDate;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public Guid Id { get; }

    public Guid UserId { get; }

    public string Title { get; private set; }

    public string Description { get; private set; }

    public TaskStatus Status { get; private set; }

    public DateOnly? DueDate { get; private set; }

    public DateTimeOffset CreatedAt { get; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public static Result<Task> Create(
        Guid userId,
        string? title,
        string? description,
        DateOnly? dueDate,
        DateTimeOffset utcNow)
    {
        var validation = TaskValidator.ValidateCreate(userId, title, description);
        if (validation.IsFailure)
        {
            return Result<Task>.FromFailure(validation);
        }

        var task = new Task(
            Guid.NewGuid(),
            userId,
            TaskValidator.NormalizeRequiredText(title),
            TaskValidator.NormalizeRequiredText(description),
            TaskStatus.Todo,
            dueDate,
            utcNow,
            utcNow);

        return Result<Task>.Success(task);
    }

    public Result Update(
        Guid requesterUserId,
        string? title,
        string? description,
        TaskStatus status,
        DateOnly? dueDate,
        DateOnly today,
        DateTimeOffset utcNow)
    {
        var ownerCheck = EnsureOwner(requesterUserId);
        if (ownerCheck.IsFailure)
        {
            return ownerCheck;
        }

        var dueDateCheck = EnsureCanStillBeUpdated(today);
        if (dueDateCheck.IsFailure)
        {
            return dueDateCheck;
        }

        var validation = TaskValidator.ValidateRequiredFields(title, description);
        if (validation.IsFailure)
        {
            return validation;
        }

        Title = TaskValidator.NormalizeRequiredText(title);
        Description = TaskValidator.NormalizeRequiredText(description);
        Status = status;
        DueDate = dueDate;
        UpdatedAt = utcNow;

        return Result.Success();
    }

    public Result ChangeStatus(
        Guid requesterUserId,
        TaskStatus status,
        DateOnly today,
        DateTimeOffset utcNow)
    {
        var ownerCheck = EnsureOwner(requesterUserId);
        if (ownerCheck.IsFailure)
        {
            return ownerCheck;
        }

        var dueDateCheck = EnsureCanStillBeUpdated(today);
        if (dueDateCheck.IsFailure)
        {
            return dueDateCheck;
        }

        Status = status;
        UpdatedAt = utcNow;

        return Result.Success();
    }

    private Result EnsureOwner(Guid requesterUserId)
    {
        if (requesterUserId != UserId)
        {
            return Result.Failure("Only the task owner can update this task.", ErrorType.Forbidden);
        }

        return Result.Success();
    }

    private Result EnsureCanStillBeUpdated(DateOnly today)
    {
        if (DueDate.HasValue && DueDate.Value <= today)
        {
            return Result.Failure("The due date has been met, so this task can no longer be updated.", ErrorType.Conflict);
        }

        return Result.Success();
    }
}
