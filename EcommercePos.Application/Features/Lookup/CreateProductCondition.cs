using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Lookup;

public static class CreateProductCondition
{
    public sealed record Request(string ConditionCode, string DisplayName);
    public sealed record Response(string ConditionCode, string DisplayName);

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
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Request request, CancellationToken ct)
        {
            var exists = await _context.ProductConditions.AnyAsync(c => c.ConditionCode == request.ConditionCode, ct);
            if (exists)
                return Result<Response>.Failure(Error.Conflict($"Product condition '{request.ConditionCode}' already exists."));

            var entity = new ProductConditions
            {
                ConditionCode = request.ConditionCode,
                DisplayName = request.DisplayName
            };

            _context.ProductConditions.Add(entity);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(entity.ConditionCode, entity.DisplayName));
        }
    }
}
