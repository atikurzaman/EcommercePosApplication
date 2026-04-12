using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Product;

public static class ToggleProductFeatured
{
    public sealed record Command(Guid ProductId);
    public sealed record Response(Guid Id, bool IsFeatured);
    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;
        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var product = await _context.Products.FindAsync(new object[] { command.ProductId }, ct);
            if (product == null || product.IsDeleted)
                return Result<Response>.Failure(Error.NotFound("Product not found"));
            product.IsFeatured = !product.IsFeatured;
            product.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);
            return Result<Response>.Success(new Response(product.Id, product.IsFeatured));
        }
    }
}