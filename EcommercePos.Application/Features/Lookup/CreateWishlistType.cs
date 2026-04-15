using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Lookup;

public static class CreateWishlistType
{
    public sealed record Request(string TypeCode, string DisplayName);
    public sealed record Response(string TypeCode, string DisplayName);

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

        public async Task<Result<Response>> Handle(Request request, CancellationToken ct)
        {
            var exists = await _context.WishlistTypes.AnyAsync(c => c.TypeCode == request.TypeCode, ct);
            if (exists)
                return Result<Response>.Failure(Error.Conflict($"Wishlist type '{request.TypeCode}' already exists."));

            var entity = new WishlistTypes
            {
                TypeCode = request.TypeCode,
                DisplayName = request.DisplayName
            };

            _context.WishlistTypes.Add(entity);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(entity.TypeCode, entity.DisplayName));
        }
    }
}
