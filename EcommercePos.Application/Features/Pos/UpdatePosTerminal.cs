using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Pos;

// ── UpdatePosTerminal ──────────────────────────────────────────────────────────
public static class UpdatePosTerminal
{
    public sealed record Request(
        string TerminalCode, string TerminalName,
        string? MachineName, string? Ipaddress, string? PrinterName, bool IsActive);

    public sealed record Command(
        Guid Id, string TerminalCode, string TerminalName,
        string? MachineName, string? Ipaddress, string? PrinterName, bool IsActive);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.TerminalCode).NotEmpty().MaximumLength(30);
            RuleFor(x => x.TerminalName).NotEmpty().MaximumLength(100);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<GetPosTerminalById.Response>> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.PosTerminals
                .FirstOrDefaultAsync(t => t.Id == command.Id && !t.IsDeleted, ct);

            if (entity == null)
                return Result<GetPosTerminalById.Response>.Failure(Error.NotFound("POS terminal not found."));

            if (entity.TerminalCode != command.TerminalCode)
            {
                var codeExists = await _context.PosTerminals
                    .AnyAsync(t => t.PosCounterId == entity.PosCounterId
                        && t.TerminalCode == command.TerminalCode && t.Id != command.Id && !t.IsDeleted, ct);
                if (codeExists)
                    return Result<GetPosTerminalById.Response>.Failure(
                        Error.Conflict($"Terminal code '{command.TerminalCode}' already exists for this counter."));
            }

            entity.TerminalCode = command.TerminalCode;
            entity.TerminalName = command.TerminalName;
            entity.MachineName = command.MachineName;
            entity.Ipaddress = command.Ipaddress;
            entity.PrinterName = command.PrinterName;
            entity.IsActive = command.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            var counterName = await _context.PosCounters
                .Where(c => c.Id == entity.PosCounterId)
                .Select(c => c.CounterName)
                .FirstOrDefaultAsync(ct) ?? string.Empty;

            return Result<GetPosTerminalById.Response>.Success(
                new GetPosTerminalById.Response(
                    entity.Id, entity.PosCounterId, counterName,
                    entity.TerminalCode, entity.TerminalName,
                    entity.MachineName, entity.Ipaddress, entity.PrinterName, entity.IsActive));
        }
    }
}
