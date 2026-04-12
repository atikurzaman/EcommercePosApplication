using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Application.Features.Product;
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
            [AsParameters] GetProducts.Query query,
            GetProducts.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(query, ct)).ToPagedResult())
            .WithName("GetProducts")
            .WithSummary("Get paginated products");

        group.MapGet("/{id:guid}", async (
            Guid id,
            GetProductById.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new GetProductById.Query(id), ct)).ToHttpResult())
            .WithName("GetProductById")
            .WithSummary("Get product by id");

        group.MapPost("/", async (
            [FromBody] CreateProduct.Command command,
            CreateProduct.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(command, ct)).ToCreatedResult("/api/products"))
            .WithName("CreateProduct")
            .WithSummary("Create a new product");

        group.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] UpdateProductBody body,
            UpdateProduct.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new UpdateProduct.Command(
                id, body.ProductCode, body.Name, body.ShortDescription, body.Description,
                body.ProductType, body.CostPrice, body.SalePrice, body.OriginalPrice,
                body.IsTaxInclusive, body.IsFeatured, body.IsActive,
                body.CategoryId, body.BrandId, body.UnitId, body.Sku, body.Barcode), ct)).ToHttpResult())
            .WithName("UpdateProduct")
            .WithSummary("Update product");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            DeleteProduct.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new DeleteProduct.Command(id), ct)).ToNoContentResult())
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
            Results.Ok(new { data = new[] { "STANDARD", "VARIANT", "BUNDLE", "DIGITAL", "SERVICE" } }))
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
            GetProductVariants.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new GetProductVariants.Request(id), ct)).ToHttpResult())
            .WithName("GetProductVariants")
            .WithSummary("Get product variants");

        group.MapPost("/{id:guid}/variants", async (
            Guid id,
            [FromBody] CreateProductVariant.Request body,
            CreateProductVariant.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(body with { ProductId = id }, ct)).ToCreatedResult($"/api/products/{id}/variants"))
            .WithName("CreateProductVariant")
            .WithSummary("Create a product variant");

        group.MapPut("/{id:guid}/variants/{variantId:guid}", async (
            Guid id,
            Guid variantId,
            [FromBody] UpdateProductVariant.Request body,
            UpdateProductVariant.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new UpdateProductVariant.Command(
                variantId, body.Name, body.Sku, body.Barcode,
                body.CostPrice, body.PriceModifier, body.OverridePrice,
                body.WeightKg, body.IsDefault, body.IsActive, body.SortOrder,
                body.ImageUrl, body.AttributeOptionIds), ct)).ToHttpResult())
            .WithName("UpdateProductVariant")
            .WithSummary("Update a product variant");

        group.MapDelete("/{id:guid}/variants/{variantId:guid}", async (
            Guid id,
            Guid variantId,
            DeleteProductVariant.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new DeleteProductVariant.Command(variantId), ct)).ToNoContentResult())
            .WithName("DeleteProductVariant")
            .WithSummary("Delete a product variant");

        // ── Images ─────────────────────────────────────────────────────────

        group.MapGet("/{id:guid}/images", async (
            Guid id,
            Guid? variantId,
            GetProductImages.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new GetProductImages.Query(id, variantId), ct)).ToHttpResult())
            .WithName("GetProductImages")
            .WithSummary("Get product images");

        group.MapPost("/{id:guid}/images", async (
            Guid id,
            [FromBody] AddProductImage.Request body,
            AddProductImage.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new AddProductImage.Command(
                id, body.VariantId, body.ImageUrl, body.AltText,
                body.SortOrder, body.IsPrimary), ct)).ToCreatedResult($"/api/products/{id}/images"))
            .WithName("AddProductImage")
            .WithSummary("Add a product image");

        group.MapPut("/{id:guid}/images/{imageId:guid}", async (
            Guid id,
            Guid imageId,
            [FromBody] UpdateProductImage.Request body,
            UpdateProductImage.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new UpdateProductImage.Command(
                imageId, body.AltText, body.SortOrder, body.IsPrimary), ct)).ToHttpResult())
            .WithName("UpdateProductImage")
            .WithSummary("Update a product image");

        group.MapDelete("/{id:guid}/images/{imageId:guid}", async (
            Guid id,
            Guid imageId,
            DeleteProductImage.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new DeleteProductImage.Command(imageId), ct)).ToNoContentResult())
            .WithName("DeleteProductImage")
            .WithSummary("Delete a product image");

        group.MapPut("/{id:guid}/images/reorder", async (
            Guid id,
            [FromBody] ReorderProductImages.Request body,
            ReorderProductImages.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new ReorderProductImages.Command(id, body.Orders), ct)).ToNoContentResult())
            .WithName("ReorderProductImages")
            .WithSummary("Reorder product images");

        // ── Tags ───────────────────────────────────────────────────────────

        group.MapGet("/{id:guid}/tags", async (
            Guid id,
            GetProductTags.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new GetProductTags.Query(id), ct)).ToHttpResult())
            .WithName("GetProductTags")
            .WithSummary("Get product tags");

        group.MapPut("/{id:guid}/tags", async (
            Guid id,
            [FromBody] ManageProductTagsRequest body,
            ManageProductTags.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new ManageProductTags.Command(id, body.TagIds), ct)).ToNoContentResult())
            .WithName("ManageProductTags")
            .WithSummary("Manage product tags");

        // ── Specifications ─────────────────────────────────────────────────

        group.MapGet("/{id:guid}/specifications", async (
            Guid id,
            GetProductSpecValues.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new GetProductSpecValues.Query(id), ct)).ToHttpResult())
            .WithName("GetProductSpecValues")
            .WithSummary("Get product specification values");

        group.MapPut("/{id:guid}/specifications", async (
            Guid id,
            [FromBody] ManageProductSpecsRequest body,
            ManageProductSpecValues.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new ManageProductSpecValues.Command(id, body.Values), ct)).ToNoContentResult())
            .WithName("ManageProductSpecValues")
            .WithSummary("Manage product specification values");

        // ── Suppliers ──────────────────────────────────────────────────────

        group.MapGet("/{id:guid}/suppliers", async (
            Guid id,
            GetProductSupplierLinks.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new GetProductSupplierLinks.Query(id), ct)).ToHttpResult())
            .WithName("GetProductSupplierLinks")
            .WithSummary("Get product supplier links");

        group.MapPost("/{id:guid}/suppliers", async (
            Guid id,
            [FromBody] AddProductSupplierLink.Request body,
            AddProductSupplierLink.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new AddProductSupplierLink.Command(
                id, body.SupplierId, body.SupplierSku, body.UnitCost,
                body.LeadTimeDays, body.IsPreferred, body.IsActive), ct)).ToCreatedResult($"/api/products/{id}/suppliers"))
            .WithName("AddProductSupplierLink")
            .WithSummary("Add a product supplier link");

        group.MapPut("/{id:guid}/suppliers/{linkId:guid}", async (
            Guid id,
            Guid linkId,
            [FromBody] UpdateProductSupplierLink.Request body,
            UpdateProductSupplierLink.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new UpdateProductSupplierLink.Command(
                linkId, body.SupplierSku, body.UnitCost,
                body.LeadTimeDays, body.IsPreferred, body.IsActive), ct)).ToHttpResult())
            .WithName("UpdateProductSupplierLink")
            .WithSummary("Update a product supplier link");

        group.MapDelete("/{id:guid}/suppliers/{linkId:guid}", async (
            Guid id,
            Guid linkId,
            DeleteProductSupplierLink.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new DeleteProductSupplierLink.Command(linkId), ct)).ToNoContentResult())
            .WithName("DeleteProductSupplierLink")
            .WithSummary("Delete a product supplier link");

        // ── Price History ──────────────────────────────────────────────────

        group.MapGet("/{id:guid}/price-history", async (
            Guid id,
            int pageIndex,
            int pageSize,
            GetProductPriceHistory.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new GetProductPriceHistory.Query(id, pageIndex, pageSize), ct)).ToPagedResult())
            .WithName("GetProductPriceHistory")
            .WithSummary("Get product price history");

        // ── Attributes ─────────────────────────────────────────────────────

        group.MapGet("/{id:guid}/attributes", async (
            Guid id,
            GetProductAttributeLinks.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new GetProductAttributeLinks.Request(id), ct)).ToHttpResult())
            .WithName("GetProductAttributeLinks")
            .WithSummary("Get product attribute links");

        group.MapPut("/{id:guid}/attributes", async (
            Guid id,
            [FromBody] ManageProductAttributesRequest body,
            ManageProductAttributeLinks.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new ManageProductAttributeLinks.Command(id, body.Links), ct)).ToHttpResult())
            .WithName("ManageProductAttributeLinks")
            .WithSummary("Manage product attribute links");

        // ── Bundle Components ──────────────────────────────────────────────

        group.MapGet("/{id:guid}/bundle/components", async (
            Guid id,
            GetBundleComponents.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new GetBundleComponents.Query(id), ct)).ToHttpResult())
            .WithName("GetBundleComponents")
            .WithSummary("Get bundle components");

        group.MapPut("/{id:guid}/bundle/components", async (
            Guid id,
            [FromBody] ManageBundleComponentsRequest body,
            ManageBundleComponents.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new ManageBundleComponents.Command(id, body.Components), ct)).ToHttpResult())
            .WithName("ManageBundleComponents")
            .WithSummary("Manage bundle components");

        // ── Bundle Option Groups ───────────────────────────────────────────

        group.MapGet("/{id:guid}/bundle/option-groups", async (
            Guid id,
            GetBundleOptionGroups.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new GetBundleOptionGroups.Query(id), ct)).ToHttpResult())
            .WithName("GetBundleOptionGroups")
            .WithSummary("Get bundle option groups");

        group.MapPost("/{id:guid}/bundle/option-groups", async (
            Guid id,
            [FromBody] CreateBundleOptionGroup.Request body,
            CreateBundleOptionGroup.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(body with { BundleProductId = id }, ct)).ToCreatedResult($"/api/products/{id}/bundle/option-groups"))
            .WithName("CreateBundleOptionGroup")
            .WithSummary("Create a bundle option group");

        group.MapPut("/{id:guid}/bundle/option-groups/{groupId:guid}", async (
            Guid id,
            Guid groupId,
            [FromBody] UpdateBundleOptionGroup.Command body,
            UpdateBundleOptionGroup.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(body with { Id = groupId }, ct)).ToHttpResult())
            .WithName("UpdateBundleOptionGroup")
            .WithSummary("Update a bundle option group");

        group.MapDelete("/{id:guid}/bundle/option-groups/{groupId:guid}", async (
            Guid id,
            Guid groupId,
            DeleteBundleOptionGroup.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new DeleteBundleOptionGroup.Command(groupId), ct)).ToNoContentResult())
            .WithName("DeleteBundleOptionGroup")
            .WithSummary("Delete a bundle option group");
    }
}

// ── Body records ───────────────────────────────────────────────────────────
public record UpdateProductBody(
    string? ProductCode, string Name, string? ShortDescription, string? Description,
    string? ProductType, decimal CostPrice, decimal SalePrice, decimal? OriginalPrice,
    bool IsTaxInclusive, bool IsFeatured, bool IsActive,
    Guid CategoryId, Guid? BrandId, Guid? UnitId, string? Sku, string? Barcode);

record ManageProductTagsRequest(List<Guid> TagIds);
record ManageProductSpecsRequest(List<ManageProductSpecValues.SpecValueInput> Values);
record ManageProductAttributesRequest(List<ManageProductAttributeLinks.AttributeLinkInput> Links);
record ManageBundleComponentsRequest(List<ManageBundleComponents.ComponentInput> Components);
