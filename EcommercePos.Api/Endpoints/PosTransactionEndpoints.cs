using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Pos;
using EcommercePos.Api.Extensions;
using EcommercePos.Api.Filters;

namespace EcommercePos.Api.Endpoints;

public static class PosTransactionEndpoints
{
    public static void MapPosTransactionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/pos-transactions").WithTags("POS Transactions");

        group.MapGet("/", async (
            [AsParameters] GetPosTransactions.Request request,
            [FromServices] GetPosTransactions.Handler handler,
            CancellationToken ct) =>
        {
            var query = new GetPosTransactions.Query(
                request.PageIndex, request.PageSize, request.Search,
                request.CashShiftId, request.CashierId, request.WarehouseId,
                request.Status, request.DateFrom, request.DateTo);
            var result = await handler.Handle(query, ct);
            return result.ToPagedResult();
        })
        .WithName("GetPosTransactions")
        .WithSummary("Get paginated POS transactions with filters");

        group.MapGet("/{id:guid}", async (
            Guid id,
            [FromServices] GetPosTransactionById.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetPosTransactionById.Query(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetPosTransactionById")
        .WithSummary("Get POS transaction by id");

        group.MapPost("/", async (
            [FromBody] CreatePosTransaction.Request request,
            [FromServices] CreatePosTransaction.Handler handler,
            CancellationToken ct) =>
        {
            var command = new CreatePosTransaction.Command(request);
            var result = await handler.Handle(command, ct);
            return result.ToCreatedResult($"/api/pos-transactions/{result.Value?.Id}");
        })
        .AddEndpointFilter<ValidationFilter<CreatePosTransaction.Request>>()
        .WithName("CreatePosTransaction")
        .WithSummary("Create a new POS transaction (full sale)");

        group.MapPost("/hold", async (
            [FromBody] CreateHeldTransaction.Request request,
            [FromServices] CreateHeldTransaction.Handler handler,
            CancellationToken ct) =>
        {
            var command = new CreateHeldTransaction.Command(request);
            var result = await handler.Handle(command, ct);
            return result.ToCreatedResult($"/api/pos-transactions/{result.Value?.Id}");
        })
        .WithName("CreateHeldTransaction")
        .WithSummary("Save a transaction without completing (hold)");

        group.MapPost("/{id:guid}/resume", async (
            Guid id,
            [FromBody] ResumeHeldTransactionRequest request,
            [FromServices] ResumeHeldTransaction.Handler handler,
            CancellationToken ct) =>
        {
            var command = new ResumeHeldTransaction.Command(id, request.Payments);
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .WithName("ResumeHeldTransaction")
        .WithSummary("Resume and complete a held transaction");

        group.MapPost("/{id:guid}/void", async (
            Guid id,
            [FromBody] VoidPosTransactionRequest request,
            [FromServices] VoidPosTransaction.Handler handler,
            CancellationToken ct) =>
        {
            var command = new VoidPosTransaction.Command(id, request.VoidedBy, request.VoidReason);
            var result = await handler.Handle(command, ct);
            return result.ToNoContentResult();
        })
        .WithName("VoidPosTransaction")
        .WithSummary("Void a POS transaction");
    }
}

public record ResumeHeldTransactionRequest(List<PaymentTenderInput> Payments);
public record VoidPosTransactionRequest(Guid VoidedBy, string VoidReason);
