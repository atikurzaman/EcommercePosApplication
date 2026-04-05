using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace EcommercePos.Application.Features.Unit.Queries;

public static class GetUnitById
{
    public sealed record Response
    {
        public Guid Id { get; init; }
        public string ShortName { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
        public Guid? BaseUnitId { get; set; }
        public decimal? ConversionFactor { get; set; }
        public bool IsActive { get; init; }
    }

    public sealed record Query(Guid Id);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var item = await _context.Units
                .Where(x => x.Id == query.Id && !x.IsDeleted)
                .AsNoTracking()
                .ProjectToType<Response>()
                .FirstOrDefaultAsync(ct);

            if (item == null)
            {
                return Result<Response>.Failure(Error.NotFound($"Unit with id '{query.Id}' was not found."));
            }

            return Result<Response>.Success(item);
        }
    }
}
