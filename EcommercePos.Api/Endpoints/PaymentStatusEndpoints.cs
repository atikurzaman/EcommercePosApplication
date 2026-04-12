using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Lookup;
using EcommercePos.Api.Extensions;
using EcommercePos.Api.Filters;

namespace EcommercePos.Api.Endpoints;

public static class PaymentStatusEndpoints
{
    public static void MapPaymentStatusEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/payment-statuses").WithTags("PaymentStatuses");

        group.MapGet("/", async (
            [AsParameters] GetPaymentStatuses.Request request,
            [FromServices] GetPaymentStatuses.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(request, ct);
            return result.ToPagedResult();
        })
        .WithName("GetPaymentStatuses")
        .WithSummary("Get paginated payment statuses");

        group.MapGet("/{code}", async (
            string code,
            [FromServices] GetPaymentStatusByCode.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetPaymentStatusByCode.Query(code), ct);
            return result.ToHttpResult();
        })
        .WithName("GetPaymentStatusByCode")
        .WithSummary("Get payment status by code");

        group.MapPost("/", async (
            [FromBody] CreatePaymentStatus.Request request,
            [FromServices] CreatePaymentStatus.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(request, ct);
            return result.ToCreatedResult($"/api/payment-statuses/{request.StatusCode}");
        })
        .AddEndpointFilter<ValidationFilter<CreatePaymentStatus.Request>>()
        .WithName("CreatePaymentStatus")
        .WithSummary("Create a new payment status");

        group.MapPut("/{code}", async (
            string code,
            [FromBody] UpdatePaymentStatus.Request request,
            [FromServices] UpdatePaymentStatus.Handler handler,
            CancellationToken ct) =>
        {
            var command = new UpdatePaymentStatus.Command(code, request.StatusCode, request.DisplayName);
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .AddEndpointFilter<ValidationFilter<UpdatePaymentStatus.Request>>()
        .WithName("UpdatePaymentStatus")
        .WithSummary("Update an existing payment status");

        group.MapDelete("/{code}", async (
            string code,
            [FromServices] DeletePaymentStatus.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new DeletePaymentStatus.Command(code), ct);
            return result.ToNoContentResult();
        })
        .WithName("DeletePaymentStatus")
        .WithSummary("Delete a payment status");
    }
}
