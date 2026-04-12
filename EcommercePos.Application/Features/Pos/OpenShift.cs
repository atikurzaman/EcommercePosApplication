using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Pos;

// ── OpenShift ──────────────────────────────────────────────────────────────────
public static class OpenShift
{
    public sealed record Request(
        Guid WarehouseId, Guid PosCounterId, Guid? PosTerminalId,
        Guid OpenedByUserId, Guid? OpenedByEmployeeId,
        decimal OpeningCash, string? Notes);

    public sealed record Response(Guid Id, string Status, DateTime OpeningDateTime);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.WarehouseId).NotEmpty();
            RuleFor(x => x.PosCounterId).NotEmpty();
            RuleFor(x => x.OpenedByUserId).NotEmpty();
            RuleFor(x => x.OpeningCash).GreaterThanOrEqualTo(0);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Request request, CancellationToken ct)
        {
            // Validate no other open shift exists for this counter
            var hasOpenShift = await _context.CashShifts
                .AnyAsync(s => s.PosCounterId == request.PosCounterId
                    && s.Status == "Open" && !s.IsDeleted, ct);

            if (hasOpenShift)
                return Result<Response>.Failure(
                    Error.Conflict("An open shift already exists for this counter."));

            var entity = new CashShifts
            {
                Id = Guid.NewGuid(),
                WarehouseId = request.WarehouseId,
                PosCounterId = request.PosCounterId,
                PosTerminalId = request.PosTerminalId,
                OpenedByUserId = request.OpenedByUserId,
                OpenedByEmployeeId = request.OpenedByEmployeeId,
                OpeningCash = request.OpeningCash,
                OpeningDateTime = DateTime.UtcNow,
                Status = "Open",
                TotalSalesAmount = 0,
                TotalTransactions = 0,
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.CashShifts.Add(entity);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(
                new Response(entity.Id, entity.Status, entity.OpeningDateTime));
        }
    }
}
