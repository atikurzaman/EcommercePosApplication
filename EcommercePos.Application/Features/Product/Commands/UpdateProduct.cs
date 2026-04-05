using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Product.Commands;

public static class UpdateProduct
{
    public sealed record Request
    {
        public string ProductCode { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string? ShortDescription { get; init; }
        public string? Description { get; init; }
        public string ProductType { get; init; } = "Standard";
        public decimal CostPrice { get; init; }
        public decimal SalePrice { get; init; }
        public decimal? OriginalPrice { get; init; }
        public bool IsTaxInclusive { get; init; }
        public bool IsFeatured { get; init; }
        public bool IsActive { get; init; }
        public Guid CategoryId { get; init; }
        public Guid? BrandId { get; init; }
        public Guid? UnitId { get; init; }
        public string? Sku { get; init; }
        public string? Barcode { get; init; }
    }

    public sealed record Response
    {
        public Guid Id { get; init; }
        public string? ProductCode { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? Sku { get; init; }
        public string? Barcode { get; init; }
        public string? ShortDescription { get; init; }
        public decimal CostPrice { get; init; }
        public decimal SalePrice { get; init; }
        public decimal? OriginalPrice { get; init; }
        public bool IsFeatured { get; init; }
        public bool IsActive { get; init; }
        public Guid CategoryId { get; init; }
        public Guid? BrandId { get; init; }
    }

    public sealed record Command(
        Guid Id, string ProductCode, string Name, string? ShortDescription, string? Description,
        string ProductType, decimal CostPrice, decimal SalePrice, decimal? OriginalPrice,
        bool IsTaxInclusive, bool IsFeatured, bool IsActive, Guid CategoryId,
        Guid? BrandId, Guid? UnitId, string? Sku, string? Barcode);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator() {
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

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var item = await _context.Products
                .Where(x => x.Id == command.Id && !x.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (item == null)
            {
                return Result<Response>.Failure(Error.NotFound($"Product with id '{command.Id}' was not found."));
            }

            var exists = await _context.Products
                .AnyAsync(x => x.ProductCode == command.ProductCode && x.Id != command.Id && !x.IsDeleted, ct);

            if (exists)
            {
                return Result<Response>.Failure(Error.Conflict($"Another Product with ProductCode '{command.ProductCode}' already exists."));
            }

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

            return Result<Response>.Success(item.Adapt<Response>());
        }
    }
}
