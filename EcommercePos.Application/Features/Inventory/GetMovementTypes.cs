using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Inventory;

public static class GetMovementTypes
{
    public sealed record Response(string TypeCode, string DisplayName);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<List<Response>>> Handle(CancellationToken ct)
        {
            var types = await _context.StockMovementTypes
                .Select(t => new Response(t.TypeCode, t.DisplayName))
                .ToListAsync(ct);

            return Result<List<Response>>.Success(types);
        }
    }
}
