using Microsoft.EntityFrameworkCore;
using Mapster;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Cart;

public static class CreateCart
{
    public sealed record Request(Guid? CustomerId, string? SessionId);

    public sealed record Response
    {
        public Guid Id { get; init; }
    }

    public sealed record Command(Request Request);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var cart = new Carts
            {
                Id = Guid.NewGuid(),
                CustomerId = command.Request.CustomerId,
                SessionId = command.Request.SessionId ?? Guid.NewGuid().ToString(),
                SubTotal = 0,
                DiscountAmount = 0,
                Total = 0,
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };

            _context.Carts.Add(cart);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response { Id = cart.Id });
        }
    }
}
