using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;
using Microsoft.EntityFrameworkCore;

namespace EcommercePos.Application.Features.Customer.Commands;

public static class DeleteCustomer
{
    public sealed record Command(Guid Id);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var item = await _context.Customers
                .Where(x => x.Id == command.Id && !x.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (item == null)
            {
                return Result.Failure(Error.NotFound($"Customer with id '{command.Id}' was not found."));
            }

            item.IsDeleted = true;
            item.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}