using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Pos;

// ── CreatePosTerminal ──────────────────────────────────────────────────────────
public static class CreatePosTerminal
{
    public sealed record Request(
        Guid PosCounterId, string TerminalCode, string TerminalName,
        string? MachineName, string? Ipaddress, string? PrinterName, bool IsActive);

    public sealed record Response(Guid Id, string TerminalCode, string TerminalName);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.PosCounterId).NotEmpty();
            RuleFor(x => x.TerminalCode).NotEmpty().MaximumLength(30);
            RuleFor(x => x.TerminalName).NotEmpty().MaximumLength(100);
        }
    }

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Request request, CancellationToken ct)
        {
            var counterExists = await _context.PosCounters
                .AnyAsync(c => c.Id == request.PosCounterId && !c.IsDeleted, ct);
            if (!counterExists)
                return Result<Response>.Failure(Error.NotFound("POS counter not found."));

            var codeExists = await _context.PosTerminals
                .AnyAsync(t => t.PosCounterId == request.PosCounterId
                    && t.TerminalCode == request.TerminalCode && !t.IsDeleted, ct);
            if (codeExists)
                return Result<Response>.Failure(
                    Error.Conflict($"Terminal code '{request.TerminalCode}' already exists for this counter."));

            var entity = new PosTerminals
            {
                Id = Guid.NewGuid(),
                PosCounterId = request.PosCounterId,
                TerminalCode = request.TerminalCode,
                TerminalName = request.TerminalName,
                MachineName = request.MachineName,
                Ipaddress = request.Ipaddress,
                PrinterName = request.PrinterName,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.PosTerminals.Add(entity);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(entity.Id, entity.TerminalCode, entity.TerminalName));
        }
    }
}
