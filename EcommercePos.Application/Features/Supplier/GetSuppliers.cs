using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Supplier;

public static class GetSuppliers
{
    public sealed record Query(
        int PageIndex = 0, int PageSize = 10, string? Search = null,
        string? SupplierType = null, bool? IsActive = null);

    public sealed record Response(
        Guid Id, string SupplierCode, string Name, string? CompanyName,
        string? ContactPerson, string? Phone, string? Email,
        string? SupplierType, string? PaymentTerms, int? LeadTimeDays, bool IsActive);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Query query, CancellationToken ct)
        {
            var dbQuery = _context.Suppliers.Where(s => !s.IsDeleted).AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query.Search))
                dbQuery = dbQuery.Where(s =>
                    s.Name.Contains(query.Search) ||
                    s.SupplierCode.Contains(query.Search) ||
                    (s.Phone != null && s.Phone.Contains(query.Search)));

            if (!string.IsNullOrWhiteSpace(query.SupplierType))
                dbQuery = dbQuery.Where(s => s.SupplierType == query.SupplierType);

            if (query.IsActive.HasValue)
                dbQuery = dbQuery.Where(s => s.IsActive == query.IsActive.Value);

            var totalCount = await dbQuery.CountAsync(ct);
            var items = await dbQuery
                .OrderBy(s => s.Name)
                .Skip(query.PageIndex * query.PageSize)
                .Take(query.PageSize)
                .Select(s => new Response(
                    s.Id, s.SupplierCode, s.Name, s.CompanyName, s.ContactPerson,
                    s.Phone, s.Email, s.SupplierType, s.PaymentTerms, s.LeadTimeDays, s.IsActive))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(
                new PagedResult<Response>(items, totalCount, query.PageIndex, query.PageSize));
        }
    }
}
