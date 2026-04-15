using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Lookup;

public static class GetPaymentStatusByCode
{
    public sealed record Query(string Code);
    public sealed record Response(string StatusCode, string DisplayName);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var entity = await _context.PaymentStatuses.AsNoTracking()
                .Where(c => c.StatusCode == query.Code)
                .FirstOrDefaultAsync(ct);

            if (entity == null)
                return Result<Response>.Failure(Error.NotFound("Payment status not found."));

            return Result<Response>.Success(new Response(entity.StatusCode, entity.DisplayName));
        }
    }
}
