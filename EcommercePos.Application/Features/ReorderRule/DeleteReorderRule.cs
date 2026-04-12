using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.ReorderRule;

public static class DeleteReorderRule
{
    public sealed record Command(Guid Id);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var rule = await _context.ReorderRules
                .Where(r => r.Id == command.Id && !r.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (rule is null)
                return Result.Failure(Error.NotFound($"Reorder rule '{command.Id}' was not found."));

            rule.IsDeleted = true;
            rule.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
