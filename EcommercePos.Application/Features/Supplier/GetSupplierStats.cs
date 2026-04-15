using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Supplier;

public static class GetSupplierStats
{
    public sealed record Query();
    public sealed record Response(int TotalSuppliers, int ActiveSuppliers);
    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;
        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var totalSuppliers = await _context.Suppliers.CountAsync(s => !s.IsDeleted, ct);
            var activeSuppliers = await _context.Suppliers.CountAsync(s => !s.IsDeleted && s.IsActive, ct);
            return Result<Response>.Success(new Response(totalSuppliers, activeSuppliers));
        }
    }
}