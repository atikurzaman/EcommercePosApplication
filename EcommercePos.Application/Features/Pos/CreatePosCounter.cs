using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Pos;

// ── CreatePosCounter ───────────────────────────────────────────────────────────
public static class CreatePosCounter
{
    public sealed record Request(Guid WarehouseId, string CounterCode, string CounterName, bool IsActive);
    public sealed record Response(Guid Id, string CounterCode, string CounterName);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.WarehouseId).NotEmpty();
            RuleFor(x => x.CounterCode).NotEmpty().MaximumLength(30);
            RuleFor(x => x.CounterName).NotEmpty().MaximumLength(100);
        }
    }

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Request request, CancellationToken ct)
        {
            var warehouseExists = await _context.Warehouses
                .AnyAsync(w => w.Id == request.WarehouseId && !w.IsDeleted, ct);
            if (!warehouseExists)
                return Result<Response>.Failure(Error.NotFound("Warehouse not found."));

            var codeExists = await _context.PosCounters
                .AnyAsync(c => c.WarehouseId == request.WarehouseId
                    && c.CounterCode == request.CounterCode && !c.IsDeleted, ct);
            if (codeExists)
                return Result<Response>.Failure(
                    Error.Conflict($"Counter code '{request.CounterCode}' already exists in this warehouse."));

            var entity = new PosCounters
            {
                Id = Guid.NewGuid(),
                WarehouseId = request.WarehouseId,
                CounterCode = request.CounterCode,
                CounterName = request.CounterName,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.PosCounters.Add(entity);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(entity.Id, entity.CounterCode, entity.CounterName));
        }
    }
}
