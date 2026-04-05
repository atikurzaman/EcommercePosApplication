using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Brand.Commands;

public static class UpdateBrand
{
    public sealed record Request
    {
        public string BrandCode { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
        public string? LogoUrl { get; init; }
        public string? Website { get; init; }
        public string? CountryOfOrigin { get; init; }
        public bool IsFeatured { get; init; }
        public bool IsActive { get; init; }
    }

    public sealed record Response
    {
        public Guid Id { get; init; }
        public string BrandCode { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
        public string? LogoUrl { get; init; }
        public bool IsActive { get; init; }
    }

    public sealed record Command(
        Guid Id, string BrandCode, string Name, string? Description, string? LogoUrl, 
        string? Website, string? CountryOfOrigin, bool IsFeatured, bool IsActive);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator() {
            RuleFor(x => x.BrandCode).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
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
            var item = await _context.Brands
                .Where(x => x.Id == command.Id && !x.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (item == null)
            {
                return Result<Response>.Failure(Error.NotFound($"Brand with id '{command.Id}' was not found."));
            }

            var exists = await _context.Brands
                .AnyAsync(x => x.BrandCode == command.BrandCode && x.Id != command.Id && !x.IsDeleted, ct);

            if (exists)
            {
                return Result<Response>.Failure(Error.Conflict($"Another Brand with BrandCode '{command.BrandCode}' already exists."));
            }

            item.BrandCode = command.BrandCode;
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

            return Result<Response>.Success(item.Adapt<Response>());
        }
    }
}
