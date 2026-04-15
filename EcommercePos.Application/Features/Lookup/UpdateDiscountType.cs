using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Lookup;

public static class UpdateDiscountType
{
    public sealed record Request(string TypeCode, string DisplayName);
    public sealed record Command(string OriginalCode, string TypeCode, string DisplayName);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.TypeCode).NotEmpty().MaximumLength(30);
            RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(80);
        }
    }

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<GetDiscountTypeByCode.Response>> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.DiscountTypes.FirstOrDefaultAsync(c => c.TypeCode == command.OriginalCode, ct);
            if (entity == null)
                return Result<GetDiscountTypeByCode.Response>.Failure(Error.NotFound("Discount type not found."));

            if (entity.TypeCode != command.TypeCode)
            {
                var exists = await _context.DiscountTypes.AnyAsync(c => c.TypeCode == command.TypeCode, ct);
                if (exists)
                    return Result<GetDiscountTypeByCode.Response>.Failure(Error.Conflict($"Discount type '{command.TypeCode}' already exists."));
            }

            entity.TypeCode = command.TypeCode;
            entity.DisplayName = command.DisplayName;

            await _context.SaveChangesAsync(ct);
            return Result<GetDiscountTypeByCode.Response>.Success(
                new GetDiscountTypeByCode.Response(entity.TypeCode, entity.DisplayName));
        }
    }
}
