using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Security;

public static class GetRoles
{
    public sealed record Request(int PageIndex = 0, int PageSize = 10, string? Search = null);
    public sealed record Response(Guid Id, string Name, string? Description, bool IsActive);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Request request, CancellationToken ct)
        {
            var query = _context.Roles.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(r => r.Name.Contains(request.Search));

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(r => r.Name)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(r => new Response(r.Id, r.Name, r.Description, r.IsActive))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(
                new PagedResult<Response>(items, totalCount, request.PageIndex, request.PageSize));
        }
    }
}
