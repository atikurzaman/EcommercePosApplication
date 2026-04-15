using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Unit;

public static class CreateUnit
{
    public sealed record Command(
        string ShortName, string Name, string? Description,
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
            var exists = await _context.Units
                .AnyAsync(u => u.ShortName == command.ShortName && !u.IsDeleted, ct);

            if (exists)
                return Result<Response>.Failure(Error.Conflict($"Unit with short name '{command.ShortName}' already exists."));

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

            return Result<Response>.Success(new Response(
                item.Id, item.ShortName, item.Name, item.Description, item.IsActive));
        }
    }
}
