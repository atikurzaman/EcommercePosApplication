using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Product.Commands;

public static class CreateProduct
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
        public string ProductCode { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
    }

    public sealed record Command(
        string ProductCode, string Name, string? ShortDescription, string? Description,
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
            var exists = await _context.Products
                .AnyAsync(x => x.ProductCode == command.ProductCode && !x.IsDeleted, ct);

            if (exists)
            {
                return Result<Response>.Failure(Error.Conflict($"Product with Code '{command.ProductCode}' already exists."));
            }

            var item = new Products
            {
                Id = Guid.NewGuid(),
                ProductCode = command.ProductCode,
                Name = command.Name,
                ShortDescription = command.ShortDescription,
                Description = command.Description,
                ProductType = command.ProductType,
                CostPrice = command.CostPrice,
                SalePrice = command.SalePrice,
                OriginalPrice = command.OriginalPrice,
                IsTaxInclusive = command.IsTaxInclusive,
                IsFeatured = command.IsFeatured,
                IsActive = command.IsActive,
                CategoryId = command.CategoryId,
                BrandId = command.BrandId,
                UnitId = command.UnitId,
                Sku = command.Sku,
                Barcode = command.Barcode,
                Slug = command.Name.ToLower().Replace(" ", "-"),
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.Products.Add(item);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(item.Adapt<Response>());
        }
    }
}
