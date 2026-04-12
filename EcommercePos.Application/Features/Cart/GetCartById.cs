using EcommercePos.Application.Services;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;
using MediatR;

namespace EcommercePos.Application.Features.Cart;

public record GetCartByIdQuery(Guid Id) : IRequest<Result<Carts>>;

public class GetCartByIdHandler : IRequestHandler<GetCartByIdQuery, Result<Carts>>
{
    private readonly ICartService _cartService;

    public GetCartByIdHandler(ICartService cartService)
    {
        _cartService = cartService;
    }

    public async Task<Result<Carts>> Handle(GetCartByIdQuery request, CancellationToken ct)
    {
        return await _cartService.GetCartByIdAsync(request.Id, ct);
    }
}
