using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Cart;

public static class CreateCart
{
    public sealed record Command(Guid? CustomerId, Guid? UserId, string? SessionId);

    public sealed record Response(
        Guid Id, Guid? CustomerId, Guid? UserId, string? SessionId,
        decimal SubTotal, decimal DiscountAmount, decimal Total);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x).Must(c => c.CustomerId.HasValue || c.UserId.HasValue || !string.IsNullOrWhiteSpace(c.SessionId))
                .WithMessage("At least one of CustomerId, UserId, or SessionId must be provided.");
        }
    }

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var cart = new Carts
            {
                Id = Guid.NewGuid(),
                CustomerId = command.CustomerId,
                UserId = command.UserId,
                SessionId = command.SessionId ?? Guid.NewGuid().ToString(),
                SubTotal = 0m,
                DiscountAmount = 0m,
                Total = 0m,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.Carts.Add(cart);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(
                cart.Id, cart.CustomerId, cart.UserId, cart.SessionId,
                cart.SubTotal, cart.DiscountAmount, cart.Total));
        }
    }
}
