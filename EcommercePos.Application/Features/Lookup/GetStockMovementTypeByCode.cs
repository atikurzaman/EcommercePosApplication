using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Lookup;

public static class GetStockMovementTypeByCode
{
    public sealed record Query(string Code);
    public sealed record Response(string TypeCode, string DisplayName, bool IsInbound);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var entity = await _context.StockMovementTypes.AsNoTracking()
                .Where(c => c.TypeCode == query.Code)
                .FirstOrDefaultAsync(ct);

            if (entity == null)
                return Result<Response>.Failure(Error.NotFound("Stock movement type not found."));

            return Result<Response>.Success(new Response(entity.TypeCode, entity.DisplayName, entity.IsInbound));
        }
    }
}
