using EcommercePos.Shared.Common;

namespace EcommercePos.Shared.Common;

public class ApiResponse<T>
{
    public bool Succeeded { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();
    public int StatusCode { get; set; } = 200;
    public string? TraceId { get; set; }

    public static ApiResponse<T> Success(T data, string? message = null)
    {
        return new ApiResponse<T>
        {
            Succeeded = true,
            Data = data,
            Message = message ?? "Operation completed successfully",
            StatusCode = 200
        };
    }

    public static ApiResponse<T> Created(T data, string? message = null)
    {
        return new ApiResponse<T>
        {
            Succeeded = true,
            Data = data,
            Message = message ?? "Resource created successfully",
            StatusCode = 201
        };
    }

    public static ApiResponse<T> Fail(string message, int statusCode = 400, List<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            Succeeded = false,
            Message = message,
            StatusCode = statusCode,
            Errors = errors ?? new List<string>()
        };
    }

    public static ApiResponse<T> NotFound(string message = "Resource not found")
    {
        return Fail(message, 404);
    }

    public static ApiResponse<T> Unauthorized(string message = "Unauthorized access")
    {
        return Fail(message, 401);
    }

    public static ApiResponse<T> Forbidden(string message = "Access forbidden")
    {
        return Fail(message, 403);
    }

    public static ApiResponse<T> ValidationError(List<string> errors)
    {
        return new ApiResponse<T>
        {
            Succeeded = false,
            Message = "Validation failed",
            StatusCode = 422,
            Errors = errors
        };
    }
}

public class ApiResponse : ApiResponse<object>
{
    public static new ApiResponse Success(string? message = null)
    {
        return new ApiResponse
        {
            Succeeded = true,
            Message = message ?? "Operation completed successfully",
            StatusCode = 200
        };
    }

    public static new ApiResponse Created(string? message = null)
    {
        return new ApiResponse
        {
            Succeeded = true,
            Message = message ?? "Resource created successfully",
            StatusCode = 201
        };
    }

    public static new ApiResponse Fail(string message, int statusCode = 400)
    {
        return new ApiResponse
        {
            Succeeded = false,
            Message = message,
            StatusCode = statusCode
        };
    }

    public static new ApiResponse NotFound(string message = "Resource not found")
    {
        return Fail(message, 404);
    }

    public static new ApiResponse Unauthorized(string message = "Unauthorized access")
    {
        return Fail(message, 401);
    }
}

