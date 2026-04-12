using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.ReorderRule;

public static class ToggleReorderRuleActive
{
    public sealed record Command(Guid Id);

    public sealed record Response(Guid Id, bool IsActive);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var rule = await _context.ReorderRules
                .Where(r => r.Id == command.Id && !r.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (rule is null)
                return Result<Response>.Failure(Error.NotFound($"Reorder rule '{command.Id}' was not found."));

            rule.IsActive = !rule.IsActive;
            rule.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(rule.Id, rule.IsActive));
        }
    }
}
