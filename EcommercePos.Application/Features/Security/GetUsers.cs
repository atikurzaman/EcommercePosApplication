using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Security;

public static class GetUsers
{
    public sealed record Request(int PageIndex = 0, int PageSize = 10, string? Search = null, bool? IsActive = null, Guid? RoleId = null);

    public sealed record Response(
        Guid Id, string UserName, string Email, string? FirstName, string? LastName,
        string? PhoneNumber, bool IsActive, bool EmailConfirmed,
        DateTime CreatedAt, DateTime? LastLoginAt, List<string> Roles);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Request request, CancellationToken ct)
        {
            var query = _context.Users.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search;
                query = query.Where(u =>
                    u.UserName.Contains(search) ||
                    u.Email.Contains(search) ||
                    (u.FirstName != null && u.FirstName.Contains(search)) ||
                    (u.LastName != null && u.LastName.Contains(search)));
            }

            if (request.IsActive.HasValue)
                query = query.Where(u => u.IsActive == request.IsActive.Value);

            if (request.RoleId.HasValue)
            {
                var roleId = request.RoleId.Value;
                query = query.Where(u =>
                    _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == roleId));
            }

            var totalCount = await query.CountAsync(ct);

            var items = await query
                .OrderBy(u => u.UserName)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(u => new Response(
                    u.Id, u.UserName, u.Email, u.FirstName, u.LastName,
                    u.PhoneNumber, u.IsActive, u.EmailConfirmed,
                    u.CreatedAt, u.LastLoginAt,
                    _context.UserRoles
                        .Where(ur => ur.UserId == u.Id)
                        .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                        .ToList()))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(
                new PagedResult<Response>(items, totalCount, request.PageIndex, request.PageSize));
        }
    }
}
