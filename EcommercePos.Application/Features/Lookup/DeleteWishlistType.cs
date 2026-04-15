using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Lookup;

public static class DeleteWishlistType
{
    public sealed record Command(string Code);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.WishlistTypes.FirstOrDefaultAsync(c => c.TypeCode == command.Code, ct);
            if (entity == null)
                return Result.Failure(Error.NotFound("Wishlist type not found."));

            _context.WishlistTypes.Remove(entity);
            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
