using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Lookup;

public static class UpdatePaymentMethod
{
    public sealed record Request(string MethodCode, string DisplayName, bool IsOnline, bool IsActive, byte SortOrder);
    public sealed record Command(string OriginalCode, string MethodCode, string DisplayName, bool IsOnline, bool IsActive, byte SortOrder);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.MethodCode).NotEmpty().MaximumLength(40);
            RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(80);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<GetPaymentMethodByCode.Response>> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.PaymentMethods.FirstOrDefaultAsync(c => c.MethodCode == command.OriginalCode, ct);
            if (entity == null)
                return Result<GetPaymentMethodByCode.Response>.Failure(Error.NotFound("Payment method not found."));

            if (entity.MethodCode != command.MethodCode)
            {
                var exists = await _context.PaymentMethods.AnyAsync(c => c.MethodCode == command.MethodCode, ct);
                if (exists)
                    return Result<GetPaymentMethodByCode.Response>.Failure(Error.Conflict($"Payment method '{command.MethodCode}' already exists."));
            }

            entity.MethodCode = command.MethodCode;
            entity.DisplayName = command.DisplayName;
            entity.IsOnline = command.IsOnline;
            entity.IsActive = command.IsActive;
            entity.SortOrder = command.SortOrder;

            await _context.SaveChangesAsync(ct);
            return Result<GetPaymentMethodByCode.Response>.Success(
                new GetPaymentMethodByCode.Response(entity.MethodCode, entity.DisplayName, entity.IsOnline, entity.IsActive, entity.SortOrder));
        }
    }
}
