namespace TaksManagementAI.API.Domain;

public enum ErrorType
{
    None,
    Validation,
    NotFound,
    Forbidden,
    Conflict
}

public sealed record Result(bool IsSuccess, string? Error, ErrorType ErrorType)
{
    public bool IsFailure => !IsSuccess;

    public static Result Success() => new(true, null, ErrorType.None);

    public static Result Failure(string error, ErrorType errorType = ErrorType.Validation)
        => new(false, error, errorType);
}

public sealed record Result<T>(bool IsSuccess, T? Value, string? Error, ErrorType ErrorType)
{
    public bool IsFailure => !IsSuccess;

    public static Result<T> Success(T value) => new(true, value, null, ErrorType.None);

    public static Result<T> Failure(string error, ErrorType errorType = ErrorType.Validation)
        => new(false, default, error, errorType);

    public static Result<T> FromFailure(Result result)
        => Failure(result.Error ?? "Operation failed.", result.ErrorType);
}
