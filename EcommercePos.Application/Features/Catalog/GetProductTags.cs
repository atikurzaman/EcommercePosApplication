using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Catalog;

public static class GetProductTags
{
    public sealed record Request(Guid ProductId);

    public sealed record Response
    {
        public Guid TagId { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Slug { get; init; } = string.Empty;
    }

    public sealed record Query(Guid ProductId);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<List<Response>>> Handle(Query query, CancellationToken ct)
        {
            var items = await _context.ProductTags
                .Where(x => x.ProductId == query.ProductId)
                .Join(
                    _context.Tags.Where(t => !t.IsDeleted),
                    pt => pt.TagId,
                    t => t.Id,
                    (pt, t) => new Response
                    {
                        TagId = t.Id,
                        Name = t.Name,
                        Slug = t.Slug
                    })
                .ToListAsync(ct);

            return Result<List<Response>>.Success(items);
        }
    }
}
