using EcommercePos.Application.Services;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;
using MediatR;

namespace EcommercePos.Application.Features.Cart;

public record GetCartsQuery : IRequest<Result<List<Carts>>>;

public class GetCartsHandler : IRequestHandler<GetCartsQuery, Result<List<Carts>>>
{
    private readonly ICartService _cartService;

    public GetCartsHandler(ICartService cartService)
    {
        _cartService = cartService;
    }

    public async Task<Result<List<Carts>>> Handle(GetCartsQuery request, CancellationToken ct)
    {
        return await _cartService.GetCartsAsync(ct);
    }
}
