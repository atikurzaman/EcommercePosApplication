using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Branch;

public static class GetBranchById
{
    public sealed record Query(Guid Id);

    public sealed record Response(
        Guid Id, string WarehouseCode, string Name, string? Description,
        string? AddressLine1, string? AddressLine2, string? City, string? Area,
        string? State, string? PostalCode, string? Phone, string? Email, bool IsActive);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var item = await _context.Warehouses
                .Where(w => w.Id == query.Id && !w.IsDeleted)
                .AsNoTracking()
                .Select(w => new Response(
                    w.Id, w.Code, w.Name, w.SiteType,
                    w.AddressLine1, w.AddressLine2, w.City, w.Area,
                    w.State, w.PostalCode, w.Phone, w.Email, w.IsActive))
                .FirstOrDefaultAsync(ct);

            return item is null
                ? Result<Response>.Failure(Error.NotFound($"Branch '{query.Id}' was not found."))
                : Result<Response>.Success(item);
        }
    }
}
