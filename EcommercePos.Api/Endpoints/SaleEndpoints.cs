using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Sale;
using EcommercePos.Api.Extensions;
using EcommercePos.Api.Filters;

namespace EcommercePos.Api.Endpoints;

public static class SaleEndpoints
{
    public static void MapSaleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/sales").WithTags("Sales");

        group.MapGet("/", async (
            [AsParameters] GetSales.Query request,
            GetSales.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(request, ct)).ToPagedResult())
        .WithName("GetSales")
        .WithSummary("Get paginated sales");

        group.MapGet("/{id:guid}", async (
            Guid id,
            GetSaleById.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new GetSaleById.Query(id), ct)).ToHttpResult())
        .WithName("GetSaleById")
        .WithSummary("Get sale by id");

        group.MapPost("/", async (
            [FromBody] CreateSale.Command command,
            CreateSale.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(command, ct)).ToCreatedResult("/api/sales"))
        .AddEndpointFilter<ValidationFilter<CreateSale.Command>>()
        .WithName("CreateSale")
        .WithSummary("Create a new sale");

        group.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] UpdateSale.Command command,
            UpdateSale.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new UpdateSale.Command(
                id,
                command.SubTotal,
                command.DiscountAmount,
                command.TaxAmount,
                command.TotalAmount,
                command.PaidAmount,
                command.StatusCode), ct)).ToHttpResult())
        .WithName("UpdateSale")
        .WithSummary("Update an existing sale");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            DeleteSale.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new DeleteSale.Command(id), ct)).ToNoContentResult())
        .WithName("DeleteSale")
        .WithSummary("Soft delete a sale");
    }
}
