using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Catalog;

public static class CreateAttributeOption
{
    public sealed record Request(
        Guid AttributeTypeId, string Value, string? DisplayValue,
        Guid? ColorId, int SortOrder, bool IsActive);

    public sealed record Response(Guid Id, string Value);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.AttributeTypeId).NotEmpty();
            RuleFor(x => x.Value).NotEmpty().MaximumLength(200);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Request request, CancellationToken ct)
        {
            var typeExists = await _context.AttributeTypes
                .AnyAsync(a => a.Id == request.AttributeTypeId && !a.IsDeleted, ct);

            if (!typeExists)
                return Result<Response>.Failure(Error.NotFound("Attribute type not found."));

            var entity = new AttributeOptions
            {
                Id = Guid.NewGuid(),
                AttributeTypeId = request.AttributeTypeId,
                Value = request.Value,
                DisplayValue = request.DisplayValue,
                ColorId = request.ColorId,
                SortOrder = request.SortOrder,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.AttributeOptions.Add(entity);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(entity.Id, entity.Value));
        }
    }
}
