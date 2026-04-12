using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Supplier;

public static class ToggleSupplierActive
{
    public sealed record Command(Guid SupplierId);
    public sealed record Response(Guid Id, bool IsActive);
    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;
        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var supplier = await _context.Suppliers.FindAsync(new object[] { command.SupplierId }, ct);
            if (supplier == null || supplier.IsDeleted)
                return Result<Response>.Failure(Error.NotFound("Supplier not found"));
            supplier.IsActive = !supplier.IsActive;
            supplier.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);
            return Result<Response>.Success(new Response(supplier.Id, supplier.IsActive));
        }
    }
}