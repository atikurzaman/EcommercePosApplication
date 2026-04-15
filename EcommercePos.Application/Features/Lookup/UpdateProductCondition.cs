using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Lookup;

public static class UpdateProductCondition
{
    public sealed record Request(string ConditionCode, string DisplayName);
    public sealed record Command(string OriginalCode, string ConditionCode, string DisplayName);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.ConditionCode).NotEmpty().MaximumLength(30);
            RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(80);
        }
    }

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<GetProductConditionByCode.Response>> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.ProductConditions.FirstOrDefaultAsync(c => c.ConditionCode == command.OriginalCode, ct);
            if (entity == null)
                return Result<GetProductConditionByCode.Response>.Failure(Error.NotFound("Product condition not found."));

            if (entity.ConditionCode != command.ConditionCode)
            {
                var exists = await _context.ProductConditions.AnyAsync(c => c.ConditionCode == command.ConditionCode, ct);
                if (exists)
                    return Result<GetProductConditionByCode.Response>.Failure(Error.Conflict($"Product condition '{command.ConditionCode}' already exists."));
            }

            entity.ConditionCode = command.ConditionCode;
            entity.DisplayName = command.DisplayName;

            await _context.SaveChangesAsync(ct);
            return Result<GetProductConditionByCode.Response>.Success(
                new GetProductConditionByCode.Response(entity.ConditionCode, entity.DisplayName));
        }
    }
}
