using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Brand;

public static class UpdateBrand
{
    public sealed record Command(
        Guid Id, string Name, string? BrandCode, string? Description, string? LogoUrl,
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
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var item = await _context.Brands
                .Where(x => x.Id == command.Id && !x.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (item is null)
                return Result<Response>.Failure(Error.NotFound($"Brand '{command.Id}' was not found."));

            var cleaned = command.Name.ToUpper().Replace(" ", "").Replace("-", "");
            var brandCode = command.BrandCode ?? cleaned[..Math.Min(10, cleaned.Length)];

            var exists = await _context.Brands
                .AnyAsync(x => x.BrandCode == brandCode && x.Id != command.Id && !x.IsDeleted, ct);

            if (exists)
                return Result<Response>.Failure(Error.Conflict($"Another brand with code '{brandCode}' already exists."));

            item.BrandCode = brandCode;
            item.Name = command.Name;
            item.Description = command.Description;
            item.LogoUrl = command.LogoUrl;
            item.Website = command.Website;
            item.CountryOfOrigin = command.CountryOfOrigin;
            item.IsFeatured = command.IsFeatured;
            item.IsActive = command.IsActive;
            item.Slug = command.Name.ToLower().Replace(" ", "-");
            item.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);
            return Result<Response>.Success(new Response(
                item.Id, item.BrandCode, item.Name, item.Description, item.LogoUrl, item.IsActive));
        }
    }
}
