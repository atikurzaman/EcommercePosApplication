using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Lookup;

public static class GetPaymentMethodByCode
{
    public sealed record Query(string Code);
    public sealed record Response(string MethodCode, string DisplayName, bool IsOnline, bool IsActive, byte SortOrder);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var entity = await _context.PaymentMethods.AsNoTracking()
                .Where(c => c.MethodCode == query.Code)
                .FirstOrDefaultAsync(ct);

            if (entity == null)
                return Result<Response>.Failure(Error.NotFound("Payment method not found."));

            return Result<Response>.Success(new Response(entity.MethodCode, entity.DisplayName, entity.IsOnline, entity.IsActive, entity.SortOrder));
        }
    }
}
