using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Catalog;

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
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

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
