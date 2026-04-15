using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Product;

public static class CreateProduct
{
    public sealed record Command(
        string ProductCode, string Name, string? ShortDescription, string? Description,
        string ProductType, decimal CostPrice, decimal SalePrice, decimal? OriginalPrice,
        bool IsTaxInclusive, bool IsFeatured, bool IsActive, Guid CategoryId,
        Guid? BrandId, Guid? UnitId, string? Sku, string? Barcode);

    public sealed record Response(Guid Id, string ProductCode, string Name);

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
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var exists = await _context.Products
                .AnyAsync(x => x.ProductCode == command.ProductCode && !x.IsDeleted, ct);

            if (exists)
                return Result<Response>.Failure(Error.Conflict($"Product with code '{command.ProductCode}' already exists."));

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

            return Result<Response>.Success(new Response(item.Id, item.ProductCode, item.Name));
        }
    }
}
