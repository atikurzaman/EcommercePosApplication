using Microsoft.AspNetCore.Http;
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
}
