using FluentValidation.Results;
using EcommercePos.Shared.Common;

namespace EcommercePos.Api.Extensions;

public static class EndpointExtensions
{
    public static IResult ToHttpResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
            return Results.Ok(new { data = result.Value });

        return result.Error?.Code switch
        {
            "validation" => Results.BadRequest(new { result.Error, result.ValidationErrors }),
            "not_found" => Results.NotFound(new { result.Error }),
            "conflict" => Results.Conflict(new { result.Error }),
            _ => Results.Problem(result.Error?.Message)
        };
    }

    public static IResult ToHttpResult(this Result result)
    {
        if (result.IsSuccess)
            return Results.Ok();

        return result.Error?.Code switch
        {
            "validation" => Results.BadRequest(new { result.Error }),
            "not_found" => Results.NotFound(new { result.Error }),
            "conflict" => Results.Conflict(new { result.Error }),
            _ => Results.Problem(result.Error?.Message)
        };
    }

    public static IResult ToCreatedResult<T>(this Result<T> result, string location)
    {
        if (result.IsSuccess)
            return Results.Created(location, new { data = result.Value });

        return result.Error?.Code switch
        {
            "validation" => Results.BadRequest(new { result.Error }),
            "not_found" => Results.NotFound(new { result.Error }),
            "conflict" => Results.Conflict(new { result.Error }),
            _ => Results.Problem(result.Error?.Message)
        };
    }

    public static IResult ToNoContentResult(this Result result)
    {
        if (result.IsSuccess)
            return Results.NoContent();

        return result.Error?.Code switch
        {
            "validation" => Results.BadRequest(new { result.Error }),
            "not_found" => Results.NotFound(new { result.Error }),
            "conflict" => Results.Conflict(new { result.Error }),
            _ => Results.Problem(result.Error?.Message)
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