using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Pos;

// ── GetWarehouseById ───────────────────────────────────────────────────────────
public static class GetWarehouseById
{
    public sealed record Query(Guid Id);

    public sealed record PosCounterInfo(Guid Id, string CounterCode, string CounterName, bool IsActive);

    public sealed record Response(
        Guid Id, string Code, string Name, string SiteType,
        Guid? ParentId, string? ContactPerson, string? ManagerName,
        string? AddressLine1, string? AddressLine2, string? City, string? Area,
        string? State, string? PostalCode, string Country,
        string? Phone, string? Email,
        decimal? Latitude, decimal? Longitude,
        TimeOnly? OpeningTime, TimeOnly? ClosingTime,
        string? TaxNumber, bool IsDefault, bool IsActive,
        List<PosCounterInfo> Counters);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var entity = await _context.Warehouses
                .AsNoTracking()
                .Where(w => w.Id == query.Id && !w.IsDeleted)
                .Select(w => new Response(
                    w.Id, w.Code, w.Name, w.SiteType,
                    w.ParentId, w.ContactPerson, w.ManagerName,
                    w.AddressLine1, w.AddressLine2, w.City, w.Area,
                    w.State, w.PostalCode, w.Country,
                    w.Phone, w.Email,
                    w.Latitude, w.Longitude,
                    w.OpeningTime, w.ClosingTime,
                    w.TaxNumber, w.IsDefault, w.IsActive,
                    w.PosCounters
                        .Where(c => !c.IsDeleted)
                        .OrderBy(c => c.CounterCode)
                        .Select(c => new PosCounterInfo(c.Id, c.CounterCode, c.CounterName, c.IsActive))
                        .ToList()))
                .FirstOrDefaultAsync(ct);

            if (entity == null)
                return Result<Response>.Failure(Error.NotFound("Warehouse not found."));

            return Result<Response>.Success(entity);
        }
    }
}
