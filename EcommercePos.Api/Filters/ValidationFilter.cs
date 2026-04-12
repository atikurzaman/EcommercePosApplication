using FluentValidation;
using EcommercePos.Shared.Common;

namespace EcommercePos.Api.Filters;

public sealed class ValidationFilter<T> : IEndpointFilter where T : class
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();
        if (validator == null)
            return await next(context);

        var argument = context.Arguments.OfType<T>().FirstOrDefault();
        if (argument == null)
            return await next(context);

        var validationResult = await validator.ValidateAsync(argument);
        if (validationResult.IsValid)
            return await next(context);

        var errors = validationResult.Errors
            .Select(e => e.ErrorMessage)
            .ToList();

        return Results.BadRequest(ApiResponse<object>.Fail(
            "Validation failed", errors));
    }
}
