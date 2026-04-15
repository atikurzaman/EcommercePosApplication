using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Catalog;

public static class BulkCreateAttributeOptions
{
    public sealed record OptionInput(string Value, string? DisplayValue, Guid? ColorId, int SortOrder, bool IsActive);

    public sealed record Request(Guid AttributeTypeId, List<OptionInput> Options);

    public sealed record Response(int CreatedCount);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.AttributeTypeId).NotEmpty();
            RuleFor(x => x.Options).NotEmpty();
            RuleForEach(x => x.Options).ChildRules(option =>
            {
                option.RuleFor(o => o.Value).NotEmpty().MaximumLength(200);
            });
        }
    }

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Request request, CancellationToken ct)
        {
            var typeExists = await _context.AttributeTypes
                .AnyAsync(a => a.Id == request.AttributeTypeId && !a.IsDeleted, ct);

            if (!typeExists)
                return Result<Response>.Failure(Error.NotFound("Attribute type not found."));

            foreach (var opt in request.Options)
            {
                _context.AttributeOptions.Add(new AttributeOptions
                {
                    Id = Guid.NewGuid(),
                    AttributeTypeId = request.AttributeTypeId,
                    Value = opt.Value,
                    DisplayValue = opt.DisplayValue,
                    ColorId = opt.ColorId,
                    SortOrder = opt.SortOrder,
                    IsActive = opt.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                });
            }

            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(request.Options.Count));
        }
    }
}
