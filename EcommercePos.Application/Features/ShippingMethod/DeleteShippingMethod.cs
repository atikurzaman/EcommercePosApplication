using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.ShippingMethod;

public static class DeleteShippingMethod
{
    public sealed record Command(Guid Id);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var method = await _context.ShippingMethods
                .Where(s => s.Id == command.Id && !s.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (method is null)
                return Result.Failure(Error.NotFound($"Shipping method '{command.Id}' was not found."));

            method.IsDeleted = true;
            method.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
