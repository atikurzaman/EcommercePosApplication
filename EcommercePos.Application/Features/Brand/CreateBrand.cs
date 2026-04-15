using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Brand;

public static class CreateBrand
{
    public sealed record Command(
        string Name, string? BrandCode, string? Description, string? LogoUrl,
        string? Website, string? CountryOfOrigin, bool IsFeatured, bool IsActive);

    public sealed record Response(Guid Id, string BrandCode, string Name, string? Description,
        string? LogoUrl, bool IsActive);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var cleaned = command.Name.ToUpper().Replace(" ", "").Replace("-", "");
            var brandCode = command.BrandCode ?? cleaned[..Math.Min(10, cleaned.Length)];

            var exists = await _context.Brands
                .AnyAsync(x => x.BrandCode == brandCode && !x.IsDeleted, ct);

            if (exists)
                return Result<Response>.Failure(Error.Conflict($"Brand with code '{brandCode}' already exists."));

            var item = new Brands
            {
                Id = Guid.NewGuid(),
                BrandCode = brandCode,
                Name = command.Name,
                Description = command.Description,
                LogoUrl = command.LogoUrl,
                Website = command.Website,
                CountryOfOrigin = command.CountryOfOrigin,
                IsFeatured = command.IsFeatured,
                IsActive = command.IsActive,
                Slug = command.Name.ToLower().Replace(" ", "-"),
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.Brands.Add(item);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(
                item.Id, item.BrandCode, item.Name, item.Description, item.LogoUrl, item.IsActive));
        }
    }
}
