using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Unit.Commands;

public static class CreateUnit
{
    public sealed record Request
    {
        public string ShortName { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
        public Guid? BaseUnitId { get; init; }
        public decimal? ConversionFactor { get; init; }
        public bool IsActive { get; init; }
    }

    public sealed record Response
    {
        public Guid Id { get; init; }
        public string ShortName { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
        public bool IsActive { get; init; }
    }

    public sealed record Command(
        string ShortName, string Name, string? Description, 
        Guid? BaseUnitId, decimal? ConversionFactor, bool IsActive);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator() {
            RuleFor(x => x.ShortName).NotEmpty();
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
            var exists = await _context.Units
                .AnyAsync(x => x.ShortName == command.ShortName && !x.IsDeleted, ct);

            if (exists)
            {
                return Result<Response>.Failure(Error.Conflict($"Unit with ShortName '{command.ShortName}' already exists."));
            }

            var item = new Units
            {
                Id = Guid.NewGuid(),
                ShortName = command.ShortName,
                Name = command.Name,
                Description = command.Description,
                BaseUnitId = command.BaseUnitId,
                ConversionFactor = command.ConversionFactor,
                IsActive = command.IsActive,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.Units.Add(item);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(item.Adapt<Response>());
        }
    }
}
