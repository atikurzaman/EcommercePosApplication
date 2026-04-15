using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Lookup;

public static class GetDiscountTypeByCode
{
    public sealed record Query(string Code);
    public sealed record Response(string TypeCode, string DisplayName);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var entity = await _context.DiscountTypes.AsNoTracking()
                .Where(c => c.TypeCode == query.Code)
                .FirstOrDefaultAsync(ct);

            if (entity == null)
                return Result<Response>.Failure(Error.NotFound("Discount type not found."));

            return Result<Response>.Success(new Response(entity.TypeCode, entity.DisplayName));
        }
    }
}
