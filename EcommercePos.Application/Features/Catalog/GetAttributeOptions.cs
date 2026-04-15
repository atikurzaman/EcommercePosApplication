using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Catalog;

public static class GetAttributeOptions
{
    public sealed record Request(Guid AttributeTypeId);

    public sealed record Response(
        Guid Id, string Value, string? DisplayValue, Guid? ColorId, int SortOrder, bool IsActive);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<List<Response>>> Handle(Request request, CancellationToken ct)
        {
            var items = await _context.AttributeOptions
                .AsNoTracking()
                .Where(o => o.AttributeTypeId == request.AttributeTypeId && !o.IsDeleted)
                .OrderBy(o => o.SortOrder)
                .Select(o => new Response(o.Id, o.Value, o.DisplayValue, o.ColorId, o.SortOrder, o.IsActive))
                .ToListAsync(ct);

            return Result<List<Response>>.Success(items);
        }
    }
}
