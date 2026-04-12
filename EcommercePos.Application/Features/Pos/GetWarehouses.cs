using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Pos;

// ── GetWarehouses ──────────────────────────────────────────────────────────────
public static class GetWarehouses
{
    public sealed record Request(int PageIndex = 0, int PageSize = 10, string? Search = null);

    public sealed record Response(
        Guid Id, string Code, string Name, string SiteType,
        string? ManagerName, string? AddressLine1, string? City,
        string? Phone, string? Email, bool IsActive, bool IsDefault);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Request request, CancellationToken ct)
        {
            var query = _context.Warehouses
                .AsNoTracking()
                .Where(w => !w.IsDeleted);

            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(w => w.Name.Contains(request.Search) || w.Code.Contains(request.Search));

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(w => w.Code)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(w => new Response(
                    w.Id, w.Code, w.Name, w.SiteType,
                    w.ManagerName, w.AddressLine1, w.City,
                    w.Phone, w.Email, w.IsActive, w.IsDefault))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(
                new PagedResult<Response>(items, totalCount, request.PageIndex, request.PageSize));
        }
    }
}
