using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.ReorderRule;

public static class UpdateReorderRule
{
    public sealed record Command(
        Guid Id, Guid? WarehouseId, Guid? PreferredSupplierId,
        decimal ReorderLevel, decimal ReorderQuantity,
        Guid? NotifyUserId, bool IsActive);

    public sealed record Response(Guid Id);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var rule = await _context.ReorderRules
                .Where(r => r.Id == command.Id && !r.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (rule is null)
                return Result<Response>.Failure(Error.NotFound($"Reorder rule '{command.Id}' was not found."));

            rule.WarehouseId = command.WarehouseId;
            rule.PreferredSupplierId = command.PreferredSupplierId;
            rule.ReorderLevel = command.ReorderLevel;
            rule.ReorderQuantity = command.ReorderQuantity;
            rule.NotifyUserId = command.NotifyUserId;
            rule.IsActive = command.IsActive;
            rule.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(rule.Id));
        }
    }
}
