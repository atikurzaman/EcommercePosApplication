using EcommercePos.Application.Services;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;
using MediatR;

namespace EcommercePos.Application.Features.Cart;

public record CreateCartCommand(Guid? CustomerId, Guid? UserId, string? SessionId) 
    : IRequest<Result<Carts>>;

public class CreateCartHandler : IRequestHandler<CreateCartCommand, Result<Carts>>
{
    private readonly ICartService _cartService;

    public CreateCartHandler(ICartService cartService)
    {
        _cartService = cartService;
    }

    public async Task<Result<Carts>> Handle(CreateCartCommand request, CancellationToken ct)
    {
        return await _cartService.CreateCartAsync(
            request.CustomerId,
            request.UserId,
            request.SessionId ?? Guid.NewGuid().ToString(),
            ct);
    }
}
