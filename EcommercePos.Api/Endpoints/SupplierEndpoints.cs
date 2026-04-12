using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Supplier;
using EcommercePos.Api.Extensions;
using EcommercePos.Api.Filters;

namespace EcommercePos.Api.Endpoints;

public static class SupplierEndpoints
{
    public static void MapSupplierEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/suppliers").WithTags("Suppliers");

        group.MapGet("/", async (
            [AsParameters] GetSuppliers.Query query,
            GetSuppliers.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(query, ct)).ToPagedResult())
            .WithName("GetSuppliers")
            .WithSummary("Get paginated suppliers");

        group.MapGet("/{id:guid}", async (
            Guid id,
            GetSupplierById.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new GetSupplierById.Query(id), ct)).ToHttpResult())
            .WithName("GetSupplierById")
            .WithSummary("Get supplier by id");

        group.MapPost("/", async (
            [FromBody] CreateSupplier.Command command,
            CreateSupplier.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(command, ct)).ToCreatedResult("/api/suppliers"))
            .AddEndpointFilter<ValidationFilter<CreateSupplier.Command>>()
            .WithName("CreateSupplier")
            .WithSummary("Create a new supplier");

        group.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] UpdateSupplier.Command body,
            UpdateSupplier.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(body with { Id = id }, ct)).ToHttpResult())
            .AddEndpointFilter<ValidationFilter<UpdateSupplier.Command>>()
            .WithName("UpdateSupplier")
            .WithSummary("Update supplier");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            DeleteSupplier.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new DeleteSupplier.Command(id), ct)).ToNoContentResult())
            .WithName("DeleteSupplier")
            .WithSummary("Soft delete supplier");

        group.MapPost("/{id:guid}/toggle-active", async (
            Guid id,
            ToggleSupplierActive.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new ToggleSupplierActive.Command(id), ct)).ToHttpResult())
            .WithName("ToggleSupplierActive")
            .WithSummary("Toggle supplier active status");

        group.MapGet("/stats", async (
            GetSupplierStats.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new GetSupplierStats.Query(), ct)).ToHttpResult())
            .WithName("GetSupplierStats")
            .WithSummary("Get supplier statistics");
    }
}
