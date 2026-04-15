using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Branch;

public static class GetBranches
{
    public sealed record Query(int PageIndex = 0, int PageSize = 10, string? Search = null);

    public sealed record Response(
        Guid Id, string WarehouseCode, string Name, string? Description,
        string? Address, string? City, string? Phone, string? Email, bool IsActive);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Query query, CancellationToken ct)
        {
            var dbQuery = _context.Warehouses.Where(w => !w.IsDeleted).AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query.Search))
                dbQuery = dbQuery.Where(w => w.Name.Contains(query.Search) || w.Code.Contains(query.Search));

            var totalCount = await dbQuery.CountAsync(ct);
            var items = await dbQuery
                .OrderBy(w => w.Name)
                .Skip(query.PageIndex * query.PageSize)
                .Take(query.PageSize)
                .Select(w => new Response(
                    w.Id, w.Code, w.Name, w.SiteType,
                    w.AddressLine1, w.City, w.Phone, w.Email, w.IsActive))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(
                new PagedResult<Response>(items, totalCount, query.PageIndex, query.PageSize));
        }
    }
}
