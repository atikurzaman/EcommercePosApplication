using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Inventory;

public static class GetMovementTypes
{
    public sealed record Response(string TypeCode, string DisplayName);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<List<Response>>> Handle(CancellationToken ct)
        {
            var types = await _context.StockMovementTypes
                .Select(t => new Response(t.TypeCode, t.DisplayName))
                .ToListAsync(ct);

            return Result<List<Response>>.Success(types);
        }
    }
}
