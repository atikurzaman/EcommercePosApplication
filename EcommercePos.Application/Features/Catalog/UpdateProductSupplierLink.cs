using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Catalog;

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
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

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
