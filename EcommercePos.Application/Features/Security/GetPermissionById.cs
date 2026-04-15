using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Security;

public static class GetPermissionById
{
    public sealed record Query(Guid Id);
    public sealed record Response(Guid Id, string PermissionCode, string Name, string Module, string? Description, bool IsActive);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var entity = await _context.Permissions.AsNoTracking()
                .Where(p => p.Id == query.Id && !p.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (entity == null)
                return Result<Response>.Failure(Error.NotFound("Permission not found."));

            return Result<Response>.Success(
                new Response(entity.Id, entity.PermissionCode, entity.Name, entity.Module, entity.Description, entity.IsActive));
        }
    }
}
