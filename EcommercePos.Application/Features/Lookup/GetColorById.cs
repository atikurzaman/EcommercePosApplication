using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Lookup;

public static class GetColorById
{
    public sealed record Query(Guid Id);
    public sealed record Response(Guid Id, string Name, string? HexCode, bool IsActive);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var entity = await _context.Colors.AsNoTracking()
                .Where(c => c.Id == query.Id)
                .FirstOrDefaultAsync(ct);

            if (entity == null)
                return Result<Response>.Failure(Error.NotFound("Color not found."));

            return Result<Response>.Success(new Response(entity.Id, entity.Name, entity.HexCode, entity.IsActive));
        }
    }
}
