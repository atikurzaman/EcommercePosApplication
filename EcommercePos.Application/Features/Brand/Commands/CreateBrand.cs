using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Brand.Commands;

public static class CreateBrand
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
        string BrandCode, string Name, string? Description, string? LogoUrl, 
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
            var exists = await _context.Brands
                .AnyAsync(x => x.BrandCode == command.BrandCode && !x.IsDeleted, ct);

            if (exists)
            {
                return Result<Response>.Failure(Error.Conflict($"Brand with BrandCode '{command.BrandCode}' already exists."));
            }

            var item = new Brands
            {
                Id = Guid.NewGuid(),
                BrandCode = command.BrandCode,
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

            return Result<Response>.Success(item.Adapt<Response>());
        }
    }
}
