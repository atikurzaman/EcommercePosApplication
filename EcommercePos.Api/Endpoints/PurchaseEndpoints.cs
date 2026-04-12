using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Purchase;
using EcommercePos.Api.Extensions;
using EcommercePos.Api.Filters;

namespace EcommercePos.Api.Endpoints;

public static class PurchaseEndpoints
{
    public static void MapPurchaseEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/purchases").WithTags("Purchases");

        group.MapGet("/", async (
            [AsParameters] GetPurchases.Query request,
            GetPurchases.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(request, ct)).ToPagedResult())
        .WithName("GetPurchases")
        .WithSummary("Get paginated purchases");

        group.MapGet("/{id:guid}", async (
            Guid id,
            GetPurchaseById.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new GetPurchaseById.Query(id), ct)).ToHttpResult())
        .WithName("GetPurchaseById")
        .WithSummary("Get purchase by id");

        group.MapPost("/", async (
            [FromBody] CreatePurchase.Command command,
            CreatePurchase.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(command, ct)).ToCreatedResult("/api/purchases"))
        .AddEndpointFilter<ValidationFilter<CreatePurchase.Command>>()
        .WithName("CreatePurchase")
        .WithSummary("Create a new purchase");

        group.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] UpdatePurchase.Command command,
            UpdatePurchase.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new UpdatePurchase.Command(
                id,
                command.Status,
                command.SubTotal,
                command.DiscountAmount,
                command.TotalTaxAmount,
                command.GrandTotal), ct)).ToHttpResult())
        .WithName("UpdatePurchase")
        .WithSummary("Update an existing purchase");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            DeletePurchase.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new DeletePurchase.Command(id), ct)).ToNoContentResult())
        .WithName("DeletePurchase")
        .WithSummary("Soft delete a purchase");
    }
}
