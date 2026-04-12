using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Application.Features.Product.Queries;
using EcommercePos.Application.Features.Product.Commands;
using EcommercePos.Application.Features.Catalog;
using EcommercePos.Api.Extensions;
using EcommercePos.Persistence.Data;

namespace EcommercePos.Api.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products").WithTags("Products");

        // ── Core CRUD ──────────────────────────────────────────────────────

        group.MapGet("/", async (
            [AsParameters] GetProducts.Request request,
            [FromServices] GetProducts.Handler handler,
            CancellationToken ct) =>
        {
            var query = new GetProducts.Query(
                request.PageIndex, request.PageSize, request.Search,
                request.CategoryId, request.BrandId);
            var result = await handler.Handle(query, ct);
            return result.ToPagedResult();
        })
        .WithName("GetProducts")
        .WithSummary("Get paginated products");

        group.MapGet("/{id:guid}", async (
            Guid id,
            [FromServices] GetProductById.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetProductById.Query(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetProductById")
        .WithSummary("Get product by id");

        group.MapPost("/", async (
            [FromBody] CreateProduct.Request request,
            [FromServices] CreateProduct.Handler handler,
            CancellationToken ct) =>
        {
            var command = new CreateProduct.Command(
                request.ProductCode, request.Name, request.ShortDescription, request.Description,
                request.ProductType, request.CostPrice, request.SalePrice, request.OriginalPrice,
                request.IsTaxInclusive, request.IsFeatured, request.IsActive, request.CategoryId,
                request.BrandId, request.UnitId, request.Sku, request.Barcode);
            var result = await handler.Handle(command, ct);
            return result.ToCreatedResult($"/api/products");
        })
        .WithName("CreateProduct")
        .WithSummary("Create a new product");

        group.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] UpdateProduct.Request request,
            [FromServices] UpdateProduct.Handler handler,
            CancellationToken ct) =>
        {
            var command = new UpdateProduct.Command(
                id, request.ProductCode, request.Name, request.ShortDescription, request.Description,
                request.ProductType, request.CostPrice, request.SalePrice, request.OriginalPrice,
                request.IsTaxInclusive, request.IsFeatured, request.IsActive, request.CategoryId,
                request.BrandId, request.UnitId, request.Sku, request.Barcode);
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .WithName("UpdateProduct")
        .WithSummary("Update product");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            [FromServices] DeleteProduct.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new DeleteProduct.Command(id), ct);
            return result.ToNoContentResult();
        })
        .WithName("DeleteProduct")
        .WithSummary("Soft delete product");

        // ── Simple utility endpoints (kept as direct DB) ───────────────────

        group.MapPost("/{id:guid}/toggle-featured", async (Guid id, ApplicationDbContext context, CancellationToken ct) =>
        {
            var product = await context.Products.FindAsync(new object[] { id }, ct);
            if (product == null || product.IsDeleted)
                return Results.NotFound(new { error = "Product not found" });

            product.IsFeatured = !product.IsFeatured;
            product.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(ct);
            return Results.Ok(new { data = new { product.Id, product.IsFeatured } });
        })
        .WithName("ToggleProductFeatured")
        .WithSummary("Toggle product featured status");

        group.MapGet("/types", (CancellationToken ct) =>
        {
            var types = new[] { "STANDARD", "VARIANT", "BUNDLE", "DIGITAL", "SERVICE" };
            return Results.Ok(new { data = types });
        })
        .WithName("GetProductTypes")
        .WithSummary("Get product types");

        group.MapGet("/stats", async (ApplicationDbContext context, CancellationToken ct) =>
        {
            var stats = new
            {
                TotalProducts = await context.Products.Where(p => !p.IsDeleted).CountAsync(ct),
                ActiveProducts = await context.Products.Where(p => !p.IsDeleted && p.IsActive).CountAsync(ct),
                FeaturedProducts = await context.Products.Where(p => !p.IsDeleted && p.IsFeatured).CountAsync(ct),
                LowStockProducts = await context.Products
                    .Include(p => p.StockItems)
                    .Where(p => !p.IsDeleted && p.StockItems.Any(s => s.QuantityOnHand <= p.ReorderLevel))
                    .CountAsync(ct)
            };

            return Results.Ok(new { data = stats });
        })
        .WithName("GetProductStats")
        .WithSummary("Get product statistics");

        // ── Variants ───────────────────────────────────────────────────────

        group.MapGet("/{id:guid}/variants", async (
            Guid id,
            [FromServices] GetProductVariants.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetProductVariants.Request(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetProductVariants")
        .WithSummary("Get product variants");

        group.MapPost("/{id:guid}/variants", async (
            Guid id,
            [FromBody] CreateProductVariant.Request body,
            [FromServices] CreateProductVariant.Handler handler,
            CancellationToken ct) =>
        {
            var request = body with { ProductId = id };
            var result = await handler.Handle(request, ct);
            return result.ToCreatedResult($"/api/products/{id}/variants");
        })
        .WithName("CreateProductVariant")
        .WithSummary("Create a product variant");

        group.MapPut("/{id:guid}/variants/{variantId:guid}", async (
            Guid id,
            Guid variantId,
            [FromBody] UpdateProductVariant.Request body,
            [FromServices] UpdateProductVariant.Handler handler,
            CancellationToken ct) =>
        {
            var command = new UpdateProductVariant.Command(
                variantId, body.Name, body.Sku, body.Barcode,
                body.CostPrice, body.PriceModifier, body.OverridePrice,
                body.WeightKg, body.IsDefault, body.IsActive, body.SortOrder,
                body.ImageUrl, body.AttributeOptionIds);
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .WithName("UpdateProductVariant")
        .WithSummary("Update a product variant");

        group.MapDelete("/{id:guid}/variants/{variantId:guid}", async (
            Guid id,
            Guid variantId,
            [FromServices] DeleteProductVariant.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new DeleteProductVariant.Command(variantId), ct);
            return result.ToNoContentResult();
        })
        .WithName("DeleteProductVariant")
        .WithSummary("Delete a product variant");

        // ── Images ─────────────────────────────────────────────────────────

        group.MapGet("/{id:guid}/images", async (
            Guid id,
            Guid? variantId,
            [FromServices] GetProductImages.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetProductImages.Query(id, variantId), ct);
            return result.ToHttpResult();
        })
        .WithName("GetProductImages")
        .WithSummary("Get product images");

        group.MapPost("/{id:guid}/images", async (
            Guid id,
            [FromBody] AddProductImage.Request body,
            [FromServices] AddProductImage.Handler handler,
            CancellationToken ct) =>
        {
            var command = new AddProductImage.Command(
                id, body.VariantId, body.ImageUrl, body.AltText,
                body.SortOrder, body.IsPrimary);
            var result = await handler.Handle(command, ct);
            return result.ToCreatedResult($"/api/products/{id}/images");
        })
        .WithName("AddProductImage")
        .WithSummary("Add a product image");

        group.MapPut("/{id:guid}/images/{imageId:guid}", async (
            Guid id,
            Guid imageId,
            [FromBody] UpdateProductImage.Request body,
            [FromServices] UpdateProductImage.Handler handler,
            CancellationToken ct) =>
        {
            var command = new UpdateProductImage.Command(
                imageId, body.AltText, body.SortOrder, body.IsPrimary);
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .WithName("UpdateProductImage")
        .WithSummary("Update a product image");

        group.MapDelete("/{id:guid}/images/{imageId:guid}", async (
            Guid id,
            Guid imageId,
            [FromServices] DeleteProductImage.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new DeleteProductImage.Command(imageId), ct);
            return result.ToNoContentResult();
        })
        .WithName("DeleteProductImage")
        .WithSummary("Delete a product image");

        group.MapPut("/{id:guid}/images/reorder", async (
            Guid id,
            [FromBody] ReorderProductImages.Request body,
            [FromServices] ReorderProductImages.Handler handler,
            CancellationToken ct) =>
        {
            var command = new ReorderProductImages.Command(id, body.Orders);
            var result = await handler.Handle(command, ct);
            return result.ToNoContentResult();
        })
        .WithName("ReorderProductImages")
        .WithSummary("Reorder product images");

        // ── Tags ───────────────────────────────────────────────────────────

        group.MapGet("/{id:guid}/tags", async (
            Guid id,
            [FromServices] GetProductTags.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetProductTags.Query(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetProductTags")
        .WithSummary("Get product tags");

        group.MapPut("/{id:guid}/tags", async (
            Guid id,
            [FromBody] ManageProductTagsRequest body,
            [FromServices] ManageProductTags.Handler handler,
            CancellationToken ct) =>
        {
            var command = new ManageProductTags.Command(id, body.TagIds);
            var result = await handler.Handle(command, ct);
            return result.ToNoContentResult();
        })
        .WithName("ManageProductTags")
        .WithSummary("Manage product tags");

        // ── Specifications ─────────────────────────────────────────────────

        group.MapGet("/{id:guid}/specifications", async (
            Guid id,
            [FromServices] GetProductSpecValues.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetProductSpecValues.Query(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetProductSpecValues")
        .WithSummary("Get product specification values");

        group.MapPut("/{id:guid}/specifications", async (
            Guid id,
            [FromBody] ManageProductSpecsRequest body,
            [FromServices] ManageProductSpecValues.Handler handler,
            CancellationToken ct) =>
        {
            var command = new ManageProductSpecValues.Command(id, body.Values);
            var result = await handler.Handle(command, ct);
            return result.ToNoContentResult();
        })
        .WithName("ManageProductSpecValues")
        .WithSummary("Manage product specification values");

        // ── Suppliers ──────────────────────────────────────────────────────

        group.MapGet("/{id:guid}/suppliers", async (
            Guid id,
            [FromServices] GetProductSupplierLinks.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetProductSupplierLinks.Query(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetProductSupplierLinks")
        .WithSummary("Get product supplier links");

        group.MapPost("/{id:guid}/suppliers", async (
            Guid id,
            [FromBody] AddProductSupplierLink.Request body,
            [FromServices] AddProductSupplierLink.Handler handler,
            CancellationToken ct) =>
        {
            var command = new AddProductSupplierLink.Command(
                id, body.SupplierId, body.SupplierSku, body.UnitCost,
                body.LeadTimeDays, body.IsPreferred, body.IsActive);
            var result = await handler.Handle(command, ct);
            return result.ToCreatedResult($"/api/products/{id}/suppliers");
        })
        .WithName("AddProductSupplierLink")
        .WithSummary("Add a product supplier link");

        group.MapPut("/{id:guid}/suppliers/{linkId:guid}", async (
            Guid id,
            Guid linkId,
            [FromBody] UpdateProductSupplierLink.Request body,
            [FromServices] UpdateProductSupplierLink.Handler handler,
            CancellationToken ct) =>
        {
            var command = new UpdateProductSupplierLink.Command(
                linkId, body.SupplierSku, body.UnitCost,
                body.LeadTimeDays, body.IsPreferred, body.IsActive);
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .WithName("UpdateProductSupplierLink")
        .WithSummary("Update a product supplier link");

        group.MapDelete("/{id:guid}/suppliers/{linkId:guid}", async (
            Guid id,
            Guid linkId,
            [FromServices] DeleteProductSupplierLink.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new DeleteProductSupplierLink.Command(linkId), ct);
            return result.ToNoContentResult();
        })
        .WithName("DeleteProductSupplierLink")
        .WithSummary("Delete a product supplier link");

        // ── Price History ──────────────────────────────────────────────────

        group.MapGet("/{id:guid}/price-history", async (
            Guid id,
            int pageIndex,
            int pageSize,
            [FromServices] GetProductPriceHistory.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(
                new GetProductPriceHistory.Query(id, pageIndex, pageSize), ct);
            return result.ToPagedResult();
        })
        .WithName("GetProductPriceHistory")
        .WithSummary("Get product price history");

        // ── Attributes ─────────────────────────────────────────────────────

        group.MapGet("/{id:guid}/attributes", async (
            Guid id,
            [FromServices] GetProductAttributeLinks.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetProductAttributeLinks.Request(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetProductAttributeLinks")
        .WithSummary("Get product attribute links");

        group.MapPut("/{id:guid}/attributes", async (
            Guid id,
            [FromBody] ManageProductAttributesRequest body,
            [FromServices] ManageProductAttributeLinks.Handler handler,
            CancellationToken ct) =>
        {
            var command = new ManageProductAttributeLinks.Command(id, body.Links);
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .WithName("ManageProductAttributeLinks")
        .WithSummary("Manage product attribute links");

        // ── Bundle Components ──────────────────────────────────────────────

        group.MapGet("/{id:guid}/bundle/components", async (
            Guid id,
            [FromServices] GetBundleComponents.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetBundleComponents.Query(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetBundleComponents")
        .WithSummary("Get bundle components");

        group.MapPut("/{id:guid}/bundle/components", async (
            Guid id,
            [FromBody] ManageBundleComponentsRequest body,
            [FromServices] ManageBundleComponents.Handler handler,
            CancellationToken ct) =>
        {
            var command = new ManageBundleComponents.Command(id, body.Components);
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .WithName("ManageBundleComponents")
        .WithSummary("Manage bundle components");

        // ── Bundle Option Groups ───────────────────────────────────────────

        group.MapGet("/{id:guid}/bundle/option-groups", async (
            Guid id,
            [FromServices] GetBundleOptionGroups.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetBundleOptionGroups.Query(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetBundleOptionGroups")
        .WithSummary("Get bundle option groups");

        group.MapPost("/{id:guid}/bundle/option-groups", async (
            Guid id,
            [FromBody] CreateBundleOptionGroup.Request body,
            [FromServices] CreateBundleOptionGroup.Handler handler,
            CancellationToken ct) =>
        {
            var request = body with { BundleProductId = id };
            var result = await handler.Handle(request, ct);
            return result.ToCreatedResult($"/api/products/{id}/bundle/option-groups");
        })
        .WithName("CreateBundleOptionGroup")
        .WithSummary("Create a bundle option group");

        group.MapPut("/{id:guid}/bundle/option-groups/{groupId:guid}", async (
            Guid id,
            Guid groupId,
            [FromBody] UpdateBundleOptionGroup.Command body,
            [FromServices] UpdateBundleOptionGroup.Handler handler,
            CancellationToken ct) =>
        {
            var command = body with { Id = groupId };
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .WithName("UpdateBundleOptionGroup")
        .WithSummary("Update a bundle option group");

        group.MapDelete("/{id:guid}/bundle/option-groups/{groupId:guid}", async (
            Guid id,
            Guid groupId,
            [FromServices] DeleteBundleOptionGroup.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new DeleteBundleOptionGroup.Command(groupId), ct);
            return result.ToNoContentResult();
        })
        .WithName("DeleteBundleOptionGroup")
        .WithSummary("Delete a bundle option group");
    }
}

// ── Request DTOs for manage/bulk endpoints ─────────────────────────────────
record ManageProductTagsRequest(List<Guid> TagIds);
record ManageProductSpecsRequest(List<ManageProductSpecValues.SpecValueInput> Values);
record ManageProductAttributesRequest(List<ManageProductAttributeLinks.AttributeLinkInput> Links);
record ManageBundleComponentsRequest(List<ManageBundleComponents.ComponentInput> Components);
