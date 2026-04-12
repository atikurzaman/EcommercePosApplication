namespace EcommercePos.Shared.Common;

public static class ResultExtensions
{
    public static bool IsSuccess(this Result result) => result.IsSuccess;
    public static bool IsSuccess<T>(this Result<T> result) => result.IsSuccess;
    public static T? GetValue<T>(this Result<T> result) => result.Value;
    public static Error? GetError(this Result result) => result.Error;
    public static Error? GetError<T>(this Result<T> result) => result.Error;
}