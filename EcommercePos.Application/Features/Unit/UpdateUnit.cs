using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Unit;

public static class UpdateUnit
{
    public sealed record Command(
        Guid Id, string ShortName, string Name, string? Description,
        Guid? BaseUnitId, decimal? ConversionFactor, bool IsActive);

    public sealed record Response(Guid Id, string ShortName, string Name, string? Description, bool IsActive);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.ShortName).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
        }
    }

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var item = await _context.Units
                .Where(u => u.Id == command.Id && !u.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (item is null)
                return Result<Response>.Failure(Error.NotFound($"Unit '{command.Id}' was not found."));

            var exists = await _context.Units
                .AnyAsync(u => u.ShortName == command.ShortName && u.Id != command.Id && !u.IsDeleted, ct);
            if (exists)
                return Result<Response>.Failure(Error.Conflict($"Another unit with short name '{command.ShortName}' already exists."));

            item.ShortName = command.ShortName;
            item.Name = command.Name;
            item.Description = command.Description;
            item.BaseUnitId = command.BaseUnitId;
            item.ConversionFactor = command.ConversionFactor;
            item.IsActive = command.IsActive;
            item.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);
            return Result<Response>.Success(new Response(
                item.Id, item.ShortName, item.Name, item.Description, item.IsActive));
        }
    }
}
