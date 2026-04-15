using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Catalog;

public static class UpdateAttributeOption
{
    public sealed record Request(string Value, string? DisplayValue, Guid? ColorId, int SortOrder, bool IsActive);
    public sealed record Command(Guid Id, string Value, string? DisplayValue, Guid? ColorId, int SortOrder, bool IsActive);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Value).NotEmpty().MaximumLength(200);
        }
    }

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<GetAttributeOptions.Response>> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.AttributeOptions
                .FirstOrDefaultAsync(o => o.Id == command.Id && !o.IsDeleted, ct);

            if (entity == null)
                return Result<GetAttributeOptions.Response>.Failure(Error.NotFound("Attribute option not found."));

            entity.Value = command.Value;
            entity.DisplayValue = command.DisplayValue;
            entity.ColorId = command.ColorId;
            entity.SortOrder = command.SortOrder;
            entity.IsActive = command.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            return Result<GetAttributeOptions.Response>.Success(
                new GetAttributeOptions.Response(
                    entity.Id, entity.Value, entity.DisplayValue,
                    entity.ColorId, entity.SortOrder, entity.IsActive));
        }
    }
}
