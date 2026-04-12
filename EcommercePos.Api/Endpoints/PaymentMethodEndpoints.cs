using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Lookup;
using EcommercePos.Api.Extensions;

namespace EcommercePos.Api.Endpoints;

public static class PaymentMethodEndpoints
{
    public static void MapPaymentMethodEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/payment-methods").WithTags("PaymentMethods");

        group.MapGet("/", async (
            [AsParameters] GetPaymentMethods.Request request,
            [FromServices] GetPaymentMethods.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(request, ct);
            return result.ToPagedResult();
        })
        .WithName("GetPaymentMethods")
        .WithSummary("Get paginated payment methods");

        group.MapGet("/{code}", async (
            string code,
            [FromServices] GetPaymentMethodByCode.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetPaymentMethodByCode.Query(code), ct);
            return result.ToHttpResult();
        })
        .WithName("GetPaymentMethodByCode")
        .WithSummary("Get payment method by code");

        group.MapPost("/", async (
            [FromBody] CreatePaymentMethod.Request request,
            [FromServices] CreatePaymentMethod.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(request, ct);
            return result.ToCreatedResult($"/api/payment-methods/{request.MethodCode}");
        })
        .WithName("CreatePaymentMethod")
        .WithSummary("Create a new payment method");

        group.MapPut("/{code}", async (
            string code,
            [FromBody] UpdatePaymentMethod.Request request,
            [FromServices] UpdatePaymentMethod.Handler handler,
            CancellationToken ct) =>
        {
            var command = new UpdatePaymentMethod.Command(code, request.MethodCode, request.DisplayName, request.IsOnline, request.IsActive, request.SortOrder);
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .WithName("UpdatePaymentMethod")
        .WithSummary("Update an existing payment method");

        group.MapDelete("/{code}", async (
            string code,
            [FromServices] DeletePaymentMethod.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new DeletePaymentMethod.Command(code), ct);
            return result.ToNoContentResult();
        })
        .WithName("DeletePaymentMethod")
        .WithSummary("Delete a payment method");
    }
}
