using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Lookup;

public static class UpdateStockMovementType
{
    public sealed record Request(string TypeCode, string DisplayName, bool IsInbound);
    public sealed record Command(string OriginalCode, string TypeCode, string DisplayName, bool IsInbound);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.TypeCode).NotEmpty().MaximumLength(30);
            RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(80);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<GetStockMovementTypeByCode.Response>> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.StockMovementTypes.FirstOrDefaultAsync(c => c.TypeCode == command.OriginalCode, ct);
            if (entity == null)
                return Result<GetStockMovementTypeByCode.Response>.Failure(Error.NotFound("Stock movement type not found."));

            if (entity.TypeCode != command.TypeCode)
            {
                var exists = await _context.StockMovementTypes.AnyAsync(c => c.TypeCode == command.TypeCode, ct);
                if (exists)
                    return Result<GetStockMovementTypeByCode.Response>.Failure(Error.Conflict($"Stock movement type '{command.TypeCode}' already exists."));
            }

            entity.TypeCode = command.TypeCode;
            entity.DisplayName = command.DisplayName;
            entity.IsInbound = command.IsInbound;

            await _context.SaveChangesAsync(ct);
            return Result<GetStockMovementTypeByCode.Response>.Success(
                new GetStockMovementTypeByCode.Response(entity.TypeCode, entity.DisplayName, entity.IsInbound));
        }
    }
}
