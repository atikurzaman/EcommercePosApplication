using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Lookup;

public static class UpdateWishlistType
{
    public sealed record Request(string TypeCode, string DisplayName);
    public sealed record Command(string OriginalCode, string TypeCode, string DisplayName);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.TypeCode).NotEmpty().MaximumLength(30);
            RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(80);
        }
    }

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<GetWishlistTypeByCode.Response>> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.WishlistTypes.FirstOrDefaultAsync(c => c.TypeCode == command.OriginalCode, ct);
            if (entity == null)
                return Result<GetWishlistTypeByCode.Response>.Failure(Error.NotFound("Wishlist type not found."));

            if (entity.TypeCode != command.TypeCode)
            {
                var exists = await _context.WishlistTypes.AnyAsync(c => c.TypeCode == command.TypeCode, ct);
                if (exists)
                    return Result<GetWishlistTypeByCode.Response>.Failure(Error.Conflict($"Wishlist type '{command.TypeCode}' already exists."));
            }

            entity.TypeCode = command.TypeCode;
            entity.DisplayName = command.DisplayName;

            await _context.SaveChangesAsync(ct);
            return Result<GetWishlistTypeByCode.Response>.Success(
                new GetWishlistTypeByCode.Response(entity.TypeCode, entity.DisplayName));
        }
    }
}
