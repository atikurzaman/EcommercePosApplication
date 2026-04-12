using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Purchase;

public static class DeletePurchase
{
    public sealed record Command(Guid Id);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var purchase = await _context.PurchaseOrders
                .Where(p => p.Id == command.Id && !p.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (purchase is null)
                return Result.Failure(Error.NotFound($"Purchase '{command.Id}' was not found."));

            purchase.IsDeleted = true;
            purchase.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
