using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Lookup;

public static class DeleteColor
{
    public sealed record Command(Guid Id);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.Colors.FirstOrDefaultAsync(c => c.Id == command.Id, ct);
            if (entity == null)
                return Result.Failure(Error.NotFound("Color not found."));

            entity.IsDeleted = true;
            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
