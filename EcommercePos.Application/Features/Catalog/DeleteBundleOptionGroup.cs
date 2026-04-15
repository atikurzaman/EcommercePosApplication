using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Catalog;

public static class DeleteBundleOptionGroup
{
    public sealed record Command(Guid Id);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var group = await _context.BundleOptionGroups
                .Where(g => g.Id == command.Id && !g.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (group == null)
                return Result.Failure(Error.NotFound($"Bundle option group with id '{command.Id}' was not found."));

            group.IsDeleted = true;
            group.UpdatedAt = DateTime.UtcNow;

            // Soft delete associated items
            var items = await _context.BundleOptionItems
                .Where(i => i.GroupId == command.Id && !i.IsDeleted)
                .ToListAsync(ct);

            foreach (var item in items)
            {
                item.IsDeleted = true;
                item.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
