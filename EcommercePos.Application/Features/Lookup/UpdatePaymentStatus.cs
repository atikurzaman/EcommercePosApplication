using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Lookup;

public static class UpdatePaymentStatus
{
    public sealed record Request(string StatusCode, string DisplayName);
    public sealed record Command(string OriginalCode, string StatusCode, string DisplayName);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.StatusCode).NotEmpty().MaximumLength(30);
            RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(80);
        }
    }

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<GetPaymentStatusByCode.Response>> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.PaymentStatuses.FirstOrDefaultAsync(c => c.StatusCode == command.OriginalCode, ct);
            if (entity == null)
                return Result<GetPaymentStatusByCode.Response>.Failure(Error.NotFound("Payment status not found."));

            if (entity.StatusCode != command.StatusCode)
            {
                var exists = await _context.PaymentStatuses.AnyAsync(c => c.StatusCode == command.StatusCode, ct);
                if (exists)
                    return Result<GetPaymentStatusByCode.Response>.Failure(Error.Conflict($"Payment status '{command.StatusCode}' already exists."));
            }

            entity.StatusCode = command.StatusCode;
            entity.DisplayName = command.DisplayName;

            await _context.SaveChangesAsync(ct);
            return Result<GetPaymentStatusByCode.Response>.Success(
                new GetPaymentStatusByCode.Response(entity.StatusCode, entity.DisplayName));
        }
    }
}
