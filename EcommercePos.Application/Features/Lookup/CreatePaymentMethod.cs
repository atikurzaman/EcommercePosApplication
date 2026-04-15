using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Lookup;

public static class CreatePaymentMethod
{
    public sealed record Request(string MethodCode, string DisplayName, bool IsOnline, bool IsActive, byte SortOrder);
    public sealed record Response(string MethodCode, string DisplayName);

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
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Request request, CancellationToken ct)
        {
            var exists = await _context.PaymentMethods.AnyAsync(c => c.MethodCode == request.MethodCode, ct);
            if (exists)
                return Result<Response>.Failure(Error.Conflict($"Payment method '{request.MethodCode}' already exists."));

            var entity = new PaymentMethods
            {
                MethodCode = request.MethodCode,
                DisplayName = request.DisplayName,
                IsOnline = request.IsOnline,
                IsActive = request.IsActive,
                SortOrder = request.SortOrder
            };

            _context.PaymentMethods.Add(entity);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(entity.MethodCode, entity.DisplayName));
        }
    }
}
