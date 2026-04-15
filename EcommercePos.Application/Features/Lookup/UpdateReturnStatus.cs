using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Lookup;

public static class UpdateReturnStatus
{
    public sealed record Request(string StatusCode, string DisplayName, byte SortOrder);
    public sealed record Command(string OriginalCode, string StatusCode, string DisplayName, byte SortOrder);

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
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<GetReturnStatusByCode.Response>> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.ReturnStatuses.FirstOrDefaultAsync(c => c.StatusCode == command.OriginalCode, ct);
            if (entity == null)
                return Result<GetReturnStatusByCode.Response>.Failure(Error.NotFound("Return status not found."));

            if (entity.StatusCode != command.StatusCode)
            {
                var exists = await _context.ReturnStatuses.AnyAsync(c => c.StatusCode == command.StatusCode, ct);
                if (exists)
                    return Result<GetReturnStatusByCode.Response>.Failure(Error.Conflict($"Return status '{command.StatusCode}' already exists."));
            }

            entity.StatusCode = command.StatusCode;
            entity.DisplayName = command.DisplayName;
            entity.SortOrder = command.SortOrder;

            await _context.SaveChangesAsync(ct);
            return Result<GetReturnStatusByCode.Response>.Success(
                new GetReturnStatusByCode.Response(entity.StatusCode, entity.DisplayName, entity.SortOrder));
        }
    }
}
