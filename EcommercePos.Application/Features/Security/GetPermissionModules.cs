using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Security;

public static class GetPermissionModules
{
    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

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
