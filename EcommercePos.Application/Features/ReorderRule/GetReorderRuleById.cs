using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.ReorderRule;

public static class GetReorderRuleById
{
    public sealed record Query(Guid Id);

    public sealed record Response(
        Guid Id, Guid ProductId, string ProductName, Guid? VariantId,
        Guid? WarehouseId, string? WarehouseName,
        Guid? PreferredSupplierId, string? PreferredSupplierName,
        decimal ReorderLevel, decimal ReorderQuantity,
        Guid? NotifyUserId, string? NotifyUserName,
        bool IsActive, DateTime CreatedAt);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var rule = await _context.ReorderRules
                .Include(r => r.Product)
                .Include(r => r.Warehouse)
                .Include(r => r.PreferredSupplier)
                .Include(r => r.NotifyUser)
                .Where(r => r.Id == query.Id && !r.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (rule is null)
                return Result<Response>.Failure(Error.NotFound($"Reorder rule '{query.Id}' was not found."));

            return Result<Response>.Success(new Response(
                rule.Id, rule.ProductId, rule.Product.Name, rule.VariantId,
                rule.WarehouseId, rule.Warehouse?.Name,
                rule.PreferredSupplierId, rule.PreferredSupplier?.Name,
                rule.ReorderLevel, rule.ReorderQuantity,
                rule.NotifyUserId,
                rule.NotifyUser != null ? rule.NotifyUser.FirstName + " " + rule.NotifyUser.LastName : null,
                rule.IsActive, rule.CreatedAt));
        }
    }
}
