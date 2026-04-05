using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Branch.Queries;

public static class GetBranches
{
    public sealed record Request(int PageIndex = 0, int PageSize = 10, string? Search = null);

    public sealed record Response
    {
        public Guid Id { get; init; }
        public string WarehouseCode { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
        public string? Address { get; init; }
        public string? City { get; init; }
        public string? Phone { get; init; }
        public string? Email { get; init; }
        public bool IsActive { get; init; }
    }

    public sealed record Query(int PageIndex, int PageSize, string? Search);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PagedResult<Response>>> Handle(Query query, CancellationToken ct)
        {
            var dbQuery = _context.Warehouses
                .Where(x => !x.IsDeleted)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                dbQuery = dbQuery.Where(x => x.Name.Contains(query.Search) || x.Code.Contains(query.Search));
            }

            var totalCount = await dbQuery.CountAsync(ct);
            var items = await dbQuery
                .Skip(query.PageIndex * query.PageSize)
                .Take(query.PageSize)
                .Select(x => new Response
                {
                    Id = x.Id,
                    WarehouseCode = x.Code,
                    Name = x.Name,
                    Description = x.SiteType,
                    Address = x.AddressLine1,
                    City = x.City,
                    Phone = x.Phone,
                    Email = x.Email,
                    IsActive = x.IsActive
                })
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(new PagedResult<Response>(items, totalCount, query.PageIndex, query.PageSize));
        }
    }
}