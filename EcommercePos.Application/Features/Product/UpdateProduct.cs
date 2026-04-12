using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Product;

public static class UpdateProduct
{
    public sealed record Command(
        Guid Id, string ProductCode, string Name, string? ShortDescription, string? Description,
        string ProductType, decimal CostPrice, decimal SalePrice, decimal? OriginalPrice,
        bool IsTaxInclusive, bool IsFeatured, bool IsActive, Guid CategoryId,
        Guid? BrandId, Guid? UnitId, string? Sku, string? Barcode);

    public sealed record Response(
        Guid Id, string? ProductCode, string Name, string? Sku, string? Barcode,
        string? ShortDescription, decimal CostPrice, decimal SalePrice, decimal? OriginalPrice,
        bool IsFeatured, bool IsActive, Guid CategoryId, Guid? BrandId);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.ProductCode).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.CategoryId).NotEmpty();
            RuleFor(x => x.CostPrice).GreaterThanOrEqualTo(0);
            RuleFor(x => x.SalePrice).GreaterThanOrEqualTo(0);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var item = await _context.Products
                .Where(x => x.Id == command.Id && !x.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (item is null)
                return Result<Response>.Failure(Error.NotFound($"Product '{command.Id}' was not found."));

            var exists = await _context.Products
                .AnyAsync(x => x.ProductCode == command.ProductCode && x.Id != command.Id && !x.IsDeleted, ct);

            if (exists)
                return Result<Response>.Failure(Error.Conflict($"Another product with code '{command.ProductCode}' already exists."));

            item.ProductCode = command.ProductCode;
            item.Name = command.Name;
            item.ShortDescription = command.ShortDescription;
            item.Description = command.Description;
            item.ProductType = command.ProductType;
            item.CostPrice = command.CostPrice;
            item.SalePrice = command.SalePrice;
            item.OriginalPrice = command.OriginalPrice;
            item.IsTaxInclusive = command.IsTaxInclusive;
            item.IsFeatured = command.IsFeatured;
            item.IsActive = command.IsActive;
            item.CategoryId = command.CategoryId;
            item.BrandId = command.BrandId;
            item.UnitId = command.UnitId;
            item.Sku = command.Sku;
            item.Barcode = command.Barcode;
            item.Slug = command.Name.ToLower().Replace(" ", "-");
            item.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);
            return Result<Response>.Success(new Response(
                item.Id, item.ProductCode, item.Name, item.Sku, item.Barcode,
                item.ShortDescription, item.CostPrice, item.SalePrice, item.OriginalPrice,
                item.IsFeatured, item.IsActive, item.CategoryId, item.BrandId));
        }
    }
}
