using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Catalog;
using EcommercePos.Api.Extensions;

namespace EcommercePos.Api.Endpoints;

public static class AttributeTypeEndpoints
{
    public static void MapAttributeTypeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/attribute-types").WithTags("AttributeTypes");

        // ── Attribute Types CRUD ───────────────────────────────────────────

        group.MapGet("/", async (
            [AsParameters] GetAttributeTypes.Request request,
            [FromServices] GetAttributeTypes.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(request, ct);
            return result.ToPagedResult();
        })
        .WithName("GetAttributeTypes")
        .WithSummary("Get paginated attribute types");

        group.MapGet("/{id:guid}", async (
            Guid id,
            [FromServices] GetAttributeTypeById.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetAttributeTypeById.Query(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetAttributeTypeById")
        .WithSummary("Get attribute type by id with options");

        group.MapPost("/", async (
            [FromBody] CreateAttributeType.Request request,
            [FromServices] CreateAttributeType.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(request, ct);
            return result.ToCreatedResult("/api/attribute-types");
        })
        .WithName("CreateAttributeType")
        .WithSummary("Create a new attribute type");

        group.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] UpdateAttributeType.Request body,
            [FromServices] UpdateAttributeType.Handler handler,
            CancellationToken ct) =>
        {
            var command = new UpdateAttributeType.Command(
                id, body.Name, body.Slug, body.UiType,
                body.AffectsPrice, body.AffectsSku, body.AffectsImage, body.AffectsStock,
                body.IsFilterable, body.SortOrder);
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .WithName("UpdateAttributeType")
        .WithSummary("Update an existing attribute type");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            [FromServices] DeleteAttributeType.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new DeleteAttributeType.Command(id), ct);
            return result.ToNoContentResult();
        })
        .WithName("DeleteAttributeType")
        .WithSummary("Soft delete an attribute type");

        // ── Attribute Options (sub-resource) ───────────────────────────────

        group.MapGet("/{id:guid}/options", async (
            Guid id,
            [FromServices] GetAttributeOptions.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetAttributeOptions.Request(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetAttributeOptions")
        .WithSummary("Get options for an attribute type");

        group.MapPost("/{id:guid}/options", async (
            Guid id,
            [FromBody] CreateAttributeOption.Request body,
            [FromServices] CreateAttributeOption.Handler handler,
            CancellationToken ct) =>
        {
            var request = body with { AttributeTypeId = id };
            var result = await handler.Handle(request, ct);
            return result.ToCreatedResult($"/api/attribute-types/{id}/options");
        })
        .WithName("CreateAttributeOption")
        .WithSummary("Create an attribute option");

        group.MapPost("/{id:guid}/options/bulk", async (
            Guid id,
            [FromBody] List<BulkCreateAttributeOptions.OptionInput> options,
            [FromServices] BulkCreateAttributeOptions.Handler handler,
            CancellationToken ct) =>
        {
            var request = new BulkCreateAttributeOptions.Request(id, options);
            var result = await handler.Handle(request, ct);
            return result.ToCreatedResult($"/api/attribute-types/{id}/options");
        })
        .WithName("BulkCreateAttributeOptions")
        .WithSummary("Bulk create attribute options");

        group.MapPut("/{id:guid}/options/{optionId:guid}", async (
            Guid id,
            Guid optionId,
            [FromBody] UpdateAttributeOption.Request body,
            [FromServices] UpdateAttributeOption.Handler handler,
            CancellationToken ct) =>
        {
            var command = new UpdateAttributeOption.Command(
                optionId, body.Value, body.DisplayValue, body.ColorId, body.SortOrder, body.IsActive);
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .WithName("UpdateAttributeOption")
        .WithSummary("Update an attribute option");

        group.MapDelete("/{id:guid}/options/{optionId:guid}", async (
            Guid id,
            Guid optionId,
            [FromServices] DeleteAttributeOption.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new DeleteAttributeOption.Command(optionId), ct);
            return result.ToNoContentResult();
        })
        .WithName("DeleteAttributeOption")
        .WithSummary("Soft delete an attribute option");
    }
}
