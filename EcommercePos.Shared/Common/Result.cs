namespace EcommercePos.Shared.Common;

public sealed class Result
{
    public bool IsSuccess { get; init; }
    public Error? Error { get; init; }

    public static Result Success() => new() { IsSuccess = true };
    public static Result Failure(Error error) => new() { IsSuccess = false, Error = error };
}

public sealed class Result<T>
{
    public bool IsSuccess { get; init; }
    public T? Value { get; init; }
    public Error? Error { get; init; }
    public Dictionary<string, string[]>? ValidationErrors { get; init; }

    public static Result<T> Success(T value) =>
        new() { IsSuccess = true, Value = value };

    public static Result<T> Failure(Error error) =>
        new() { IsSuccess = false, Error = error };

    public static Result<T> ValidationFailure(Dictionary<string, string[]> errors) =>
        new()
        {
            IsSuccess = false,
            Error = Error.Validation("One or more validation errors occurred."),
            ValidationErrors = errors
        };

    public static Result<T> ValidationFailure(IDictionary<string, string[]> errors) =>
        new()
        {
            IsSuccess = false,
            Error = Error.Validation("One or more validation errors occurred."),
            ValidationErrors = new Dictionary<string, string[]>(errors)
        };
}
