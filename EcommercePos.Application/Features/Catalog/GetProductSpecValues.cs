using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Catalog;

public static class GetProductSpecValues
{
    public sealed record Request(Guid ProductId);

    public sealed record Response
    {
        public Guid Id { get; init; }
        public Guid SpecId { get; init; }
        public string SpecName { get; init; } = string.Empty;
        public Guid? VariantId { get; init; }
        public string Value { get; init; } = string.Empty;
    }

    public sealed record Query(Guid ProductId);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<List<Response>>> Handle(Query query, CancellationToken ct)
        {
            var items = await _context.ProductSpecificationValues
                .Where(x => x.ProductId == query.ProductId && !x.IsDeleted)
                .Join(
                    _context.ProductSpecifications.Where(s => !s.IsDeleted),
                    v => v.SpecId,
                    s => s.Id,
                    (v, s) => new Response
                    {
                        Id = v.Id,
                        SpecId = v.SpecId,
                        SpecName = s.SpecName,
                        VariantId = v.VariantId,
                        Value = v.Value
                    })
                .ToListAsync(ct);

            return Result<List<Response>>.Success(items);
        }
    }
}
