using FluentValidation.Results;
using EcommercePos.Shared.Common;

namespace EcommercePos.Api.Extensions;

public static class EndpointExtensions
{
    public static IResult ToHttpResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
            return Results.Ok(ApiResponse<T>.Ok(result.Value!));

        return MapErrorToResult<T>(result.Error, result.ValidationErrors);
    }

    public static IResult ToHttpResult(this Result result)
    {
        if (result.IsSuccess)
            return Results.Ok(ApiResponse<object>.Ok(null!, "Operation completed successfully"));

        return MapErrorToResult<object>(result.Error);
    }

    public static IResult ToCreatedResult<T>(this Result<T> result, string location)
    {
        if (result.IsSuccess)
            return Results.Created(location, ApiResponse<T>.Created(result.Value!));

        return MapErrorToResult<T>(result.Error, result.ValidationErrors);
    }

    public static IResult ToNoContentResult(this Result result)
    {
        if (result.IsSuccess)
            return Results.NoContent();

        return MapErrorToResult<object>(result.Error);
    }

    public static IResult ToPagedResult<T>(this Result<PagedResult<T>> result)
    {
        if (result.IsSuccess)
        {
            var paged = result.Value!;
            return Results.Ok(ApiResponse<List<T>>.Ok(paged.Items, "Data retrieved successfully") with
            {
                Pagination = new PaginationInfo
                {
                    PageNumber = paged.PageIndex + 1,
                    PageSize = paged.PageSize,
                    TotalCount = paged.TotalCount,
                    TotalPages = paged.TotalPages
                }
            });
        }

        return MapErrorToResult<List<T>>(result.Error, result.ValidationErrors);
    }

    private static IResult MapErrorToResult<T>(Error? error, Dictionary<string, string[]>? validationErrors = null)
    {
        var errorMessages = validationErrors?
            .SelectMany(e => e.Value)
            .ToList() ?? new List<string>();

        if (error == null)
            return Results.Problem("An unknown error occurred.");

        return error.Code switch
        {
            "validation" => Results.BadRequest(ApiResponse<T>.Fail(
                error.Message, errorMessages.Count > 0 ? errorMessages : new List<string> { error.Message })),
            "not_found" => Results.NotFound(ApiResponse<T>.Fail(error.Message)),
            "conflict" => Results.Conflict(ApiResponse<T>.Fail(error.Message)),
            "unauthorized" => Results.Json(ApiResponse<T>.Fail(error.Message), statusCode: 401),
            "forbidden" => Results.Json(ApiResponse<T>.Fail(error.Message), statusCode: 403),
            _ => Results.Problem(error.Message)
        };
    }

    public static Dictionary<string, string[]> ToDictionary(this ValidationResult validationResult)
    {
        return validationResult.Errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.ErrorMessage).ToArray());
    }
}
