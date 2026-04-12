using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Lookup;

public static class UpdateOrderStatus
{
    public sealed record Request(string StatusCode, string DisplayName, string? Description, byte SortOrder, bool IsTerminal);
    public sealed record Command(string OriginalCode, string StatusCode, string DisplayName, string? Description, byte SortOrder, bool IsTerminal);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.StatusCode).NotEmpty().MaximumLength(30);
            RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(80);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<GetOrderStatusByCode.Response>> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.OrderStatuses.FirstOrDefaultAsync(c => c.StatusCode == command.OriginalCode, ct);
            if (entity == null)
                return Result<GetOrderStatusByCode.Response>.Failure(Error.NotFound("Order status not found."));

            if (entity.StatusCode != command.StatusCode)
            {
                var exists = await _context.OrderStatuses.AnyAsync(c => c.StatusCode == command.StatusCode, ct);
                if (exists)
                    return Result<GetOrderStatusByCode.Response>.Failure(Error.Conflict($"Order status '{command.StatusCode}' already exists."));
            }

            entity.StatusCode = command.StatusCode;
            entity.DisplayName = command.DisplayName;
            entity.Description = command.Description;
            entity.SortOrder = command.SortOrder;
            entity.IsTerminal = command.IsTerminal;

            await _context.SaveChangesAsync(ct);
            return Result<GetOrderStatusByCode.Response>.Success(
                new GetOrderStatusByCode.Response(entity.StatusCode, entity.DisplayName, entity.Description, entity.SortOrder, entity.IsTerminal));
        }
    }
}
