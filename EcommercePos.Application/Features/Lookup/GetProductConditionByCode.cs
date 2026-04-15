using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Lookup;

public static class GetProductConditionByCode
{
    public sealed record Query(string Code);
    public sealed record Response(string ConditionCode, string DisplayName);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var entity = await _context.ProductConditions.AsNoTracking()
                .Where(c => c.ConditionCode == query.Code)
                .FirstOrDefaultAsync(ct);

            if (entity == null)
                return Result<Response>.Failure(Error.NotFound("Product condition not found."));

            return Result<Response>.Success(new Response(entity.ConditionCode, entity.DisplayName));
        }
    }
}
