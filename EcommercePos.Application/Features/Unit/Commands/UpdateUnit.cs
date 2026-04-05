using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Unit.Commands;

public static class UpdateUnit
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
        Guid Id, string ShortName, string Name, string? Description, 
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
            var item = await _context.Units
                .Where(x => x.Id == command.Id && !x.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (item == null)
            {
                return Result<Response>.Failure(Error.NotFound($"Unit with id '{command.Id}' was not found."));
            }

            var exists = await _context.Units
                .AnyAsync(x => x.ShortName == command.ShortName && x.Id != command.Id && !x.IsDeleted, ct);

            if (exists)
            {
                return Result<Response>.Failure(Error.Conflict($"Another Unit with ShortName '{command.ShortName}' already exists."));
            }

            item.ShortName = command.ShortName;
            item.Name = command.Name;
            item.Description = command.Description;
            item.BaseUnitId = command.BaseUnitId;
            item.ConversionFactor = command.ConversionFactor;
            item.IsActive = command.IsActive;
            item.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(item.Adapt<Response>());
        }
    }
}
