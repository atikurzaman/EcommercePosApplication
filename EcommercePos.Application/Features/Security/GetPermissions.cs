using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Security;

public static class GetPermissions
{
    public sealed record Request(int PageIndex = 0, int PageSize = 10, string? Search = null, string? Module = null);
    public sealed record Response(Guid Id, string PermissionCode, string Name, string Module, string? Description, bool IsActive);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Request request, CancellationToken ct)
        {
            var query = _context.Permissions
                .Where(p => !p.IsDeleted)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(p =>
                    p.PermissionCode.Contains(request.Search) ||
                    p.Name.Contains(request.Search) ||
                    p.Module.Contains(request.Search));

            if (!string.IsNullOrWhiteSpace(request.Module))
                query = query.Where(p => p.Module == request.Module);

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(p => p.Module)
                .ThenBy(p => p.Name)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(p => new Response(p.Id, p.PermissionCode, p.Name, p.Module, p.Description, p.IsActive))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(
                new PagedResult<Response>(items, totalCount, request.PageIndex, request.PageSize));
        }
    }
}
