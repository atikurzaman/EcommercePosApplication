using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Lookup;

public static class DeleteCurrency
{
    public sealed record Command(string Code);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.Currencies.FirstOrDefaultAsync(c => c.CurrencyCode == command.Code, ct);
            if (entity == null)
                return Result.Failure(Error.NotFound("Currency not found."));

            _context.Currencies.Remove(entity);
            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
