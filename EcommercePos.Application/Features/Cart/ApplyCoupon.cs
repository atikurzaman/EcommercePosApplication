using EcommercePos.Application.Services;
using EcommercePos.Shared.Common;
using MediatR;

namespace EcommercePos.Application.Features.Cart;

public record ApplyCouponCommand(Guid CartId, string CouponCode) : IRequest<Result>;

public class ApplyCouponHandler : IRequestHandler<ApplyCouponCommand, Result>
{
    private readonly ICartService _cartService;

    public ApplyCouponHandler(ICartService cartService)
    {
        _cartService = cartService;
    }

    public async Task<Result> Handle(ApplyCouponCommand request, CancellationToken ct)
    {
        return await _cartService.ApplyCouponAsync(request.CartId, request.CouponCode, ct);
    }
}
