using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Inventory;

public static class UpdateReorderLevel
{
    public sealed record Command(Guid Id, decimal ReorderLevel);

    public sealed record Response(Guid Id, decimal ReorderLevel);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var item = await _context.StockItems
                .Where(s => s.Id == command.Id && !s.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (item is null)
                return Result<Response>.Failure(Error.NotFound($"Stock item '{command.Id}' was not found."));

            item.ReorderLevel = command.ReorderLevel;
            item.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(item.Id, item.ReorderLevel ?? 0));
        }
    }
}
