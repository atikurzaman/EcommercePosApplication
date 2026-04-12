using EcommercePos.Application.Services;
using EcommercePos.Shared.Common;
using MediatR;

namespace EcommercePos.Application.Features.Cart;

public record UpdateCartItemCommand(Guid ItemId, decimal Quantity) : IRequest<Result>;

public class UpdateCartItemHandler : IRequestHandler<UpdateCartItemCommand, Result>
{
    private readonly ICartService _cartService;

    public UpdateCartItemHandler(ICartService cartService)
    {
        _cartService = cartService;
    }

    public async Task<Result> Handle(UpdateCartItemCommand request, CancellationToken ct)
    {
        return await _cartService.UpdateItemQuantityAsync(request.ItemId, request.Quantity, ct);
    }
}
