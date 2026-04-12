using EcommercePos.Application.Services;
using EcommercePos.Shared.Common;
using MediatR;

namespace EcommercePos.Application.Features.Cart;

public record AddCartItemCommand(Guid CartId, Guid ProductId, decimal Quantity, decimal UnitPrice) 
    : IRequest<Result>;

public class AddCartItemHandler : IRequestHandler<AddCartItemCommand, Result>
{
    private readonly ICartService _cartService;

    public AddCartItemHandler(ICartService cartService)
    {
        _cartService = cartService;
    }

    public async Task<Result> Handle(AddCartItemCommand request, CancellationToken ct)
    {
        return await _cartService.AddItemAsync(
            request.CartId,
            request.ProductId,
            request.Quantity,
            request.UnitPrice,
            ct);
    }
}
