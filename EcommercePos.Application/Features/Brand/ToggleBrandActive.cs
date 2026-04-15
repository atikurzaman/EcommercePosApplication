using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Brand;

public static class ToggleBrandActive
{
    public sealed record Command(Guid BrandId);
    public sealed record Response(Guid Id, bool IsActive);
    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;
        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var brand = await _context.Brands.FindAsync(new object[] { command.BrandId }, ct);
            if (brand == null || brand.IsDeleted)
                return Result<Response>.Failure(Error.NotFound("Brand not found"));
            brand.IsActive = !brand.IsActive;
            brand.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);
            return Result<Response>.Success(new Response(brand.Id, brand.IsActive));
        }
    }
}