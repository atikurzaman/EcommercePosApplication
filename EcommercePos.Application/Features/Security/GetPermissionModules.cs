using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Security;

public static class GetPermissionModules
{
    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<List<string>>> Handle(CancellationToken ct)
        {
            var modules = await _context.Permissions
                .Where(p => !p.IsDeleted && p.IsActive)
                .Select(p => p.Module)
                .Distinct()
                .OrderBy(m => m)
                .ToListAsync(ct);

            return Result<List<string>>.Success(modules);
        }
    }
}
