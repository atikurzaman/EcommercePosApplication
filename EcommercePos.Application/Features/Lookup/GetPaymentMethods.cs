using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Lookup;

public static class GetPaymentMethods
{
    public sealed record Request(int PageIndex = 0, int PageSize = 10, string? Search = null);
    public sealed record Response(string MethodCode, string DisplayName, bool IsOnline, bool IsActive, byte SortOrder);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Request request, CancellationToken ct)
        {
            var query = _context.PaymentMethods.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(c => c.MethodCode.Contains(request.Search) || c.DisplayName.Contains(request.Search));

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(c => c.SortOrder)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new Response(c.MethodCode, c.DisplayName, c.IsOnline, c.IsActive, c.SortOrder))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(new PagedResult<Response>(items, totalCount, request.PageIndex, request.PageSize));
        }
    }
}
