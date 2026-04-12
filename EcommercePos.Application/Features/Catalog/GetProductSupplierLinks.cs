using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Catalog;

public static class GetProductSupplierLinks
{
    public sealed record Request(Guid ProductId);

    public sealed record Response
    {
        public Guid Id { get; init; }
        public Guid SupplierId { get; init; }
        public string SupplierName { get; init; } = string.Empty;
        public string? SupplierCode { get; init; }
        public string? SupplierSku { get; init; }
        public decimal? UnitCost { get; init; }
        public int? LeadTimeDays { get; init; }
        public bool IsPreferred { get; init; }
        public bool IsActive { get; init; }
    }

    public sealed record Query(Guid ProductId);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<List<Response>>> Handle(Query query, CancellationToken ct)
        {
            var items = await _context.ProductSupplierLinks
                .Where(x => x.ProductId == query.ProductId && !x.IsDeleted)
                .Join(
                    _context.Suppliers.Where(s => !s.IsDeleted),
                    l => l.SupplierId,
                    s => s.Id,
                    (l, s) => new Response
                    {
                        Id = l.Id,
                        SupplierId = l.SupplierId,
                        SupplierName = s.Name,
                        SupplierCode = s.SupplierCode,
                        SupplierSku = l.SupplierSku,
                        UnitCost = l.UnitCost,
                        LeadTimeDays = l.LeadTimeDays,
                        IsPreferred = l.IsPreferred,
                        IsActive = l.IsActive
                    })
                .ToListAsync(ct);

            return Result<List<Response>>.Success(items);
        }
    }
}
