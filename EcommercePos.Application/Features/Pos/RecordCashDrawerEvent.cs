using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Pos;

// ── RecordCashDrawerEvent ──────────────────────────────────────────────────────
public static class RecordCashDrawerEvent
{
    public sealed record Request(
        Guid CashShiftId, Guid PerformedBy, Guid? TransactionId,
        string EventType, decimal Amount, string? Notes);

    public sealed record Response(Guid Id, string EventType, decimal Amount, DateTime OccurredAt);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.CashShiftId).NotEmpty();
            RuleFor(x => x.PerformedBy).NotEmpty();
            RuleFor(x => x.EventType).NotEmpty().MaximumLength(20)
                .Must(v => v is "OPEN" or "CLOSE" or "CASH_IN" or "CASH_OUT" or "FLOAT" or "SALE")
                .WithMessage("EventType must be one of: OPEN, CLOSE, CASH_IN, CASH_OUT, FLOAT, SALE.");
            RuleFor(x => x.Amount).GreaterThanOrEqualTo(0);
        }
    }

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Request request, CancellationToken ct)
        {
            var shiftExists = await _context.CashShifts
                .AnyAsync(s => s.Id == request.CashShiftId && !s.IsDeleted, ct);
            if (!shiftExists)
                return Result<Response>.Failure(Error.NotFound("Cash shift not found."));

            var entity = new CashDrawerEvents
            {
                Id = Guid.NewGuid(),
                CashShiftId = request.CashShiftId,
                PerformedBy = request.PerformedBy,
                TransactionId = request.TransactionId,
                EventType = request.EventType,
                Amount = request.Amount,
                Notes = request.Notes,
                OccurredAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.CashDrawerEvents.Add(entity);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(
                new Response(entity.Id, entity.EventType, entity.Amount, entity.OccurredAt));
        }
    }
}
