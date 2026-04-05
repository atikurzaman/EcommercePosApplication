using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;
using Mapster;

namespace EcommercePos.Application.Features.Branch.Queries;

public static class GetBranchById
{
    public sealed record Response
    {
        public Guid Id { get; init; }
        public string WarehouseCode { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
        public string? AddressLine1 { get; init; }
        public string? AddressLine2 { get; init; }
        public string? City { get; init; }
        public string? Area { get; init; }
        public string? State { get; init; }
        public string? PostalCode { get; init; }
        public string? Phone { get; init; }
        public string? Email { get; init; }
        public bool IsActive { get; init; }
        public bool IsDefault { get; init; }
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
            var item = await _context.Warehouses
                .Where(x => x.Id == query.Id && !x.IsDeleted)
                .AsNoTracking()
                .Select(x => new Response
                {
                    Id = x.Id,
                    WarehouseCode = x.Code,
                    Name = x.Name,
                    Description = x.SiteType,
                    AddressLine1 = x.AddressLine1,
                    AddressLine2 = x.AddressLine2,
                    City = x.City,
                    Area = x.Area,
                    State = x.State,
                    PostalCode = x.PostalCode,
                    Phone = x.Phone,
                    Email = x.Email,
                    IsActive = x.IsActive,
                    IsDefault = x.IsDefault
                })
                .FirstOrDefaultAsync(ct);

            if (item == null)
            {
                return Result<Response>.Failure(Error.NotFound($"Warehouse with id '{query.Id}' was not found."));
            }

            return Result<Response>.Success(item);
        }
    }
}