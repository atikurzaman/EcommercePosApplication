using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Pos;

// ── GetCashShifts ──────────────────────────────────────────────────────────────
public static class GetCashShifts
{
    public sealed record Request(
        int PageIndex = 0, int PageSize = 10,
        Guid? WarehouseId = null, string? Status = null,
        DateTime? DateFrom = null, DateTime? DateTo = null);

    public sealed record Response(
        Guid Id, Guid WarehouseId, string WarehouseName,
        Guid PosCounterId, string CounterName,
        Guid? OpenedByUserId, string? OpenedByName,
        string Status, DateTime OpeningDateTime, DateTime? ClosingDateTime,
        decimal OpeningCash, decimal? ClosingCash,
        decimal TotalSalesAmount, int TotalTransactions);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Request request, CancellationToken ct)
        {
            var query = _context.CashShifts
                .AsNoTracking()
                .Where(s => !s.IsDeleted);

            if (request.WarehouseId.HasValue)
                query = query.Where(s => s.WarehouseId == request.WarehouseId.Value);
            if (!string.IsNullOrWhiteSpace(request.Status))
                query = query.Where(s => s.Status == request.Status);
            if (request.DateFrom.HasValue)
                query = query.Where(s => s.OpeningDateTime >= request.DateFrom.Value);
            if (request.DateTo.HasValue)
                query = query.Where(s => s.OpeningDateTime <= request.DateTo.Value);

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderByDescending(s => s.OpeningDateTime)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(s => new Response(
                    s.Id, s.WarehouseId, s.Warehouse.Name,
                    s.PosCounterId, s.PosCounter.CounterName,
                    s.OpenedByUserId, s.OpenedByUser != null ? (s.OpenedByUser.FirstName + " " + s.OpenedByUser.LastName) : null,
                    s.Status, s.OpeningDateTime, s.ClosingDateTime,
                    s.OpeningCash, s.ClosingCash,
                    s.TotalSalesAmount, s.TotalTransactions))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(
                new PagedResult<Response>(items, totalCount, request.PageIndex, request.PageSize));
        }
    }
}

// ── GetActiveShift ─────────────────────────────────────────────────────────────
public static class GetActiveShift
{
    public sealed record Query(Guid? UserId, Guid? WarehouseId);

    public sealed record Response(
        Guid Id, Guid WarehouseId, string WarehouseName,
        Guid PosCounterId, string CounterName,
        Guid? PosTerminalId, string? TerminalName,
        Guid? OpenedByUserId, string? OpenedByName,
        Guid? OpenedByEmployeeId,
        string Status, DateTime OpeningDateTime,
        decimal OpeningCash, decimal TotalSalesAmount, int TotalTransactions,
        string? Notes);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var q = _context.CashShifts
                .AsNoTracking()
                .Where(s => !s.IsDeleted && s.Status == "Open");

            if (query.UserId.HasValue)
                q = q.Where(s => s.OpenedByUserId == query.UserId.Value);
            if (query.WarehouseId.HasValue)
                q = q.Where(s => s.WarehouseId == query.WarehouseId.Value);

            var entity = await q
                .Select(s => new Response(
                    s.Id, s.WarehouseId, s.Warehouse.Name,
                    s.PosCounterId, s.PosCounter.CounterName,
                    s.PosTerminalId, s.PosTerminal != null ? s.PosTerminal.TerminalName : null,
                    s.OpenedByUserId, s.OpenedByUser != null ? (s.OpenedByUser.FirstName + " " + s.OpenedByUser.LastName) : null,
                    s.OpenedByEmployeeId,
                    s.Status, s.OpeningDateTime,
                    s.OpeningCash, s.TotalSalesAmount, s.TotalTransactions,
                    s.Notes))
                .FirstOrDefaultAsync(ct);

            if (entity == null)
                return Result<Response>.Failure(Error.NotFound("No active shift found."));

            return Result<Response>.Success(entity);
        }
    }
}

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

// ── GetShiftSummary ────────────────────────────────────────────────────────────
public static class GetShiftSummary
{
    public sealed record Query(Guid ShiftId);

    public sealed record TransactionInfo(
        Guid Id, string ReceiptNumber, DateTime SaleDate,
        string SaleType, decimal GrandTotal, string Status);

    public sealed record CashDrawerEventInfo(
        Guid Id, string EventType, decimal Amount,
        string? Notes, DateTime OccurredAt,
        Guid PerformedBy, string? PerformedByName);

    public sealed record PaymentBreakdown(string MethodCode, decimal TotalAmount, int Count);

    public sealed record Response(
        Guid Id, Guid WarehouseId, string WarehouseName,
        Guid PosCounterId, string CounterName,
        Guid? OpenedByUserId, string? OpenedByName,
        Guid? ClosedByUserId, string? ClosedByName,
        string Status, DateTime OpeningDateTime, DateTime? ClosingDateTime,
        decimal OpeningCash, decimal? ClosingCash,
        decimal? ExpectedCash, decimal? CashVariance,
        decimal TotalSalesAmount, int TotalTransactions,
        string? Notes,
        List<TransactionInfo> Transactions,
        List<CashDrawerEventInfo> CashDrawerEvents,
        List<PaymentBreakdown> PaymentBreakdowns);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var shift = await _context.CashShifts
                .AsNoTracking()
                .Where(s => s.Id == query.ShiftId && !s.IsDeleted)
                .Select(s => new
                {
                    s.Id, s.WarehouseId,
                    WarehouseName = s.Warehouse.Name,
                    s.PosCounterId,
                    CounterName = s.PosCounter.CounterName,
                    s.OpenedByUserId,
                    OpenedByName = s.OpenedByUser != null ? (s.OpenedByUser.FirstName + " " + s.OpenedByUser.LastName) : null,
                    s.ClosedByUserId,
                    ClosedByName = s.ClosedByUser != null ? (s.ClosedByUser.FirstName + " " + s.ClosedByUser.LastName) : null,
                    s.Status, s.OpeningDateTime, s.ClosingDateTime,
                    s.OpeningCash, s.ClosingCash,
                    s.ExpectedCash, s.CashVariance,
                    s.TotalSalesAmount, s.TotalTransactions,
                    s.Notes
                })
                .FirstOrDefaultAsync(ct);

            if (shift == null)
                return Result<Response>.Failure(Error.NotFound("Shift not found."));

            var transactions = await _context.PosTransactions
                .AsNoTracking()
                .Where(t => t.CashShiftId == query.ShiftId && !t.IsDeleted)
                .OrderByDescending(t => t.SaleDate)
                .Select(t => new TransactionInfo(
                    t.Id, t.ReceiptNumber, t.SaleDate,
                    t.SaleType, t.GrandTotal, t.Status))
                .ToListAsync(ct);

            var events = await _context.CashDrawerEvents
                .AsNoTracking()
                .Where(e => e.CashShiftId == query.ShiftId && !e.IsDeleted)
                .OrderBy(e => e.OccurredAt)
                .Select(e => new CashDrawerEventInfo(
                    e.Id, e.EventType, e.Amount,
                    e.Notes, e.OccurredAt,
                    e.PerformedBy,
                    e.PerformedByNavigation != null ? (e.PerformedByNavigation.FirstName + " " + e.PerformedByNavigation.LastName) : null))
                .ToListAsync(ct);

            var paymentBreakdowns = await _context.PosPaymentTenders
                .AsNoTracking()
                .Where(p => p.Transaction.CashShiftId == query.ShiftId && !p.IsDeleted)
                .GroupBy(p => p.MethodCode)
                .Select(g => new PaymentBreakdown(g.Key, g.Sum(p => p.Amount), g.Count()))
                .ToListAsync(ct);

            return Result<Response>.Success(new Response(
                shift.Id, shift.WarehouseId, shift.WarehouseName,
                shift.PosCounterId, shift.CounterName,
                shift.OpenedByUserId, shift.OpenedByName,
                shift.ClosedByUserId, shift.ClosedByName,
                shift.Status, shift.OpeningDateTime, shift.ClosingDateTime,
                shift.OpeningCash, shift.ClosingCash,
                shift.ExpectedCash, shift.CashVariance,
                shift.TotalSalesAmount, shift.TotalTransactions,
                shift.Notes, transactions, events, paymentBreakdowns));
        }
    }
}
