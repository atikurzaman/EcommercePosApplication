using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Sale;

public static class DeleteSale
{
    public sealed record Command(Guid Id);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var order = await _context.Orders
                .Where(o => o.Id == command.Id && !o.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (order is null)
                return Result.Failure(Error.NotFound($"Sale '{command.Id}' was not found."));

            order.IsDeleted = true;
            order.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
