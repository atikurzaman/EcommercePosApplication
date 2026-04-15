using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Unit;

public static class GetUnitById
{
    public sealed record Query(Guid Id);

    public sealed record Response(
        Guid Id, string ShortName, string Name, string? Description,
        Guid? BaseUnitId, decimal? ConversionFactor, bool IsActive);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var item = await _context.Units
                .Where(u => u.Id == query.Id && !u.IsDeleted)
                .AsNoTracking()
                .Select(u => new Response(u.Id, u.ShortName, u.Name, u.Description,
                    u.BaseUnitId, u.ConversionFactor, u.IsActive))
                .FirstOrDefaultAsync(ct);

            return item is null
                ? Result<Response>.Failure(Error.NotFound($"Unit '{query.Id}' was not found."))
                : Result<Response>.Success(item);
        }
    }
}
