using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Pos;

// ── CloseShift ─────────────────────────────────────────────────────────────────
public static class CloseShift
{
    public sealed record Command(
        Guid ShiftId, Guid ClosedByUserId, Guid? ClosedByEmployeeId,
        decimal ClosingCash, string? Notes);

    public sealed record Response(
        Guid Id, string Status, DateTime? ClosingDateTime,
        decimal OpeningCash, decimal ClosingCash,
        decimal? ExpectedCash, decimal? CashVariance);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.ShiftId).NotEmpty();
            RuleFor(x => x.ClosedByUserId).NotEmpty();
            RuleFor(x => x.ClosingCash).GreaterThanOrEqualTo(0);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.CashShifts
                .FirstOrDefaultAsync(s => s.Id == command.ShiftId && !s.IsDeleted, ct);

            if (entity == null)
                return Result<Response>.Failure(Error.NotFound("Shift not found."));

            if (entity.Status != "Open")
                return Result<Response>.Failure(
                    Error.Conflict("Only an open shift can be closed."));

            var expectedCash = entity.OpeningCash + entity.TotalSalesAmount;
            var cashVariance = command.ClosingCash - expectedCash;

            entity.ClosedByUserId = command.ClosedByUserId;
            entity.ClosedByEmployeeId = command.ClosedByEmployeeId;
            entity.ClosingCash = command.ClosingCash;
            entity.ExpectedCash = expectedCash;
            entity.CashVariance = cashVariance;
            entity.ClosingDateTime = DateTime.UtcNow;
            entity.Status = "Closed";
            entity.Notes = command.Notes ?? entity.Notes;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(
                new Response(entity.Id, entity.Status, entity.ClosingDateTime,
                    entity.OpeningCash, command.ClosingCash,
                    expectedCash, cashVariance));
        }
    }
}
