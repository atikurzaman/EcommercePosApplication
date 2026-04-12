using EcommercePos.Application.Services;
using EcommercePos.Shared.Common;
using MediatR;

namespace EcommercePos.Application.Features.Cart;

public record RemoveCartItemCommand(Guid ItemId) : IRequest<Result>;

public class RemoveCartItemHandler : IRequestHandler<RemoveCartItemCommand, Result>
{
    private readonly ICartService _cartService;

    public RemoveCartItemHandler(ICartService cartService)
    {
        _cartService = cartService;
    }

    public async Task<Result> Handle(RemoveCartItemCommand request, CancellationToken ct)
    {
        return await _cartService.RemoveItemAsync(request.ItemId, ct);
    }
}
