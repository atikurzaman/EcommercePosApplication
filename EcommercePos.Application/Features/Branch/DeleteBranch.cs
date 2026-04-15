using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Branch;

public static class DeleteBranch
{
    public sealed record Command(Guid Id);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var item = await _context.Warehouses
                .Where(w => w.Id == command.Id && !w.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (item is null)
                return Result.Failure(Error.NotFound($"Branch '{command.Id}' was not found."));

            item.IsDeleted = true;
            item.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
