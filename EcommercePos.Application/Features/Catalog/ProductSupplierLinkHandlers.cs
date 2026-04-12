using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Catalog;

// ── GetProductSupplierLinks ─────────────────────────────────────────────────
public static class GetProductSupplierLinks
{
    public sealed record Request(Guid ProductId);

    public sealed record Response
    {
        public Guid Id { get; init; }
        public Guid SupplierId { get; init; }
        public string SupplierName { get; init; } = string.Empty;
        public string? SupplierCode { get; init; }
        public string? SupplierSku { get; init; }
        public decimal? UnitCost { get; init; }
        public int? LeadTimeDays { get; init; }
        public bool IsPreferred { get; init; }
        public bool IsActive { get; init; }
    }

    public sealed record Query(Guid ProductId);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<List<Response>>> Handle(Query query, CancellationToken ct)
        {
            var items = await _context.ProductSupplierLinks
                .Where(x => x.ProductId == query.ProductId && !x.IsDeleted)
                .Join(
                    _context.Suppliers.Where(s => !s.IsDeleted),
                    l => l.SupplierId,
                    s => s.Id,
                    (l, s) => new Response
                    {
                        Id = l.Id,
                        SupplierId = l.SupplierId,
                        SupplierName = s.Name,
                        SupplierCode = s.SupplierCode,
                        SupplierSku = l.SupplierSku,
                        UnitCost = l.UnitCost,
                        LeadTimeDays = l.LeadTimeDays,
                        IsPreferred = l.IsPreferred,
                        IsActive = l.IsActive
                    })
                .ToListAsync(ct);

            return Result<List<Response>>.Success(items);
        }
    }
}

// ── AddProductSupplierLink ──────────────────────────────────────────────────
public static class AddProductSupplierLink
{
    public sealed record Request
    {
        public Guid ProductId { get; init; }
        public Guid SupplierId { get; init; }
        public string? SupplierSku { get; init; }
        public decimal? UnitCost { get; init; }
        public int? LeadTimeDays { get; init; }
        public bool IsPreferred { get; init; }
        public bool IsActive { get; init; }
    }

    public sealed record Response
    {
        public Guid Id { get; init; }
        public Guid ProductId { get; init; }
        public Guid SupplierId { get; init; }
        public string? SupplierSku { get; init; }
        public decimal? UnitCost { get; init; }
        public int? LeadTimeDays { get; init; }
        public bool IsPreferred { get; init; }
        public bool IsActive { get; init; }
    }

    public sealed record Command(
        Guid ProductId, Guid SupplierId, string? SupplierSku, decimal? UnitCost,
        int? LeadTimeDays, bool IsPreferred, bool IsActive);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.SupplierId).NotEmpty();
            RuleFor(x => x.SupplierSku).MaximumLength(200);
            RuleFor(x => x.UnitCost).GreaterThanOrEqualTo(0).When(x => x.UnitCost.HasValue);
            RuleFor(x => x.LeadTimeDays).GreaterThanOrEqualTo(0).When(x => x.LeadTimeDays.HasValue);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            if (command.IsPreferred)
            {
                var existingPreferred = await _context.ProductSupplierLinks
                    .Where(x => x.ProductId == command.ProductId
                                && x.IsPreferred && !x.IsDeleted)
                    .ToListAsync(ct);

                foreach (var p in existingPreferred)
                    p.IsPreferred = false;
            }

            var item = new ProductSupplierLinks
            {
                Id = Guid.NewGuid(),
                ProductId = command.ProductId,
                SupplierId = command.SupplierId,
                SupplierSku = command.SupplierSku,
                UnitCost = command.UnitCost,
                LeadTimeDays = command.LeadTimeDays,
                IsPreferred = command.IsPreferred,
                IsActive = command.IsActive,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.ProductSupplierLinks.Add(item);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response
            {
                Id = item.Id,
                ProductId = item.ProductId,
                SupplierId = item.SupplierId,
                SupplierSku = item.SupplierSku,
                UnitCost = item.UnitCost,
                LeadTimeDays = item.LeadTimeDays,
                IsPreferred = item.IsPreferred,
                IsActive = item.IsActive
            });
        }
    }
}

// ── UpdateProductSupplierLink ───────────────────────────────────────────────
public static class UpdateProductSupplierLink
{
    public sealed record Request
    {
        public string? SupplierSku { get; init; }
        public decimal? UnitCost { get; init; }
        public int? LeadTimeDays { get; init; }
        public bool IsPreferred { get; init; }
        public bool IsActive { get; init; }
    }

    public sealed record Response
    {
        public Guid Id { get; init; }
        public Guid ProductId { get; init; }
        public Guid SupplierId { get; init; }
        public string? SupplierSku { get; init; }
        public decimal? UnitCost { get; init; }
        public int? LeadTimeDays { get; init; }
        public bool IsPreferred { get; init; }
        public bool IsActive { get; init; }
    }

    public sealed record Command(Guid Id, string? SupplierSku, decimal? UnitCost,
        int? LeadTimeDays, bool IsPreferred, bool IsActive);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.SupplierSku).MaximumLength(200);
            RuleFor(x => x.UnitCost).GreaterThanOrEqualTo(0).When(x => x.UnitCost.HasValue);
            RuleFor(x => x.LeadTimeDays).GreaterThanOrEqualTo(0).When(x => x.LeadTimeDays.HasValue);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var item = await _context.ProductSupplierLinks
                .Where(x => x.Id == command.Id && !x.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (item == null)
                return Result<Response>.Failure(Error.NotFound($"ProductSupplierLink with id '{command.Id}' was not found."));

            if (command.IsPreferred && !item.IsPreferred)
            {
                var existingPreferred = await _context.ProductSupplierLinks
                    .Where(x => x.ProductId == item.ProductId
                                && x.Id != item.Id
                                && x.IsPreferred && !x.IsDeleted)
                    .ToListAsync(ct);

                foreach (var p in existingPreferred)
                    p.IsPreferred = false;
            }

            item.SupplierSku = command.SupplierSku;
            item.UnitCost = command.UnitCost;
            item.LeadTimeDays = command.LeadTimeDays;
            item.IsPreferred = command.IsPreferred;
            item.IsActive = command.IsActive;
            item.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response
            {
                Id = item.Id,
                ProductId = item.ProductId,
                SupplierId = item.SupplierId,
                SupplierSku = item.SupplierSku,
                UnitCost = item.UnitCost,
                LeadTimeDays = item.LeadTimeDays,
                IsPreferred = item.IsPreferred,
                IsActive = item.IsActive
            });
        }
    }
}

// ── DeleteProductSupplierLink ───────────────────────────────────────────────
public static class DeleteProductSupplierLink
{
    public sealed record Command(Guid Id);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var item = await _context.ProductSupplierLinks
                .Where(x => x.Id == command.Id && !x.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (item == null)
                return Result.Failure(Error.NotFound($"ProductSupplierLink with id '{command.Id}' was not found."));

            item.IsDeleted = true;
            item.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            return Result.Success();
        }
    }
}
