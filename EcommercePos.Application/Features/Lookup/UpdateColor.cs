using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Lookup;

public static class UpdateColor
{
    public sealed record Request(string Name, string? HexCode, bool IsActive);
    public sealed record Command(Guid Id, string Name, string? HexCode, bool IsActive);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
            RuleFor(x => x.HexCode).MaximumLength(7).Matches(@"^#[0-9A-Fa-f]{6}$").When(x => x.HexCode != null);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<GetColorById.Response>> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.Colors.FirstOrDefaultAsync(c => c.Id == command.Id, ct);
            if (entity == null)
                return Result<GetColorById.Response>.Failure(Error.NotFound("Color not found."));

            entity.Name = command.Name;
            entity.HexCode = command.HexCode;
            entity.IsActive = command.IsActive;

            await _context.SaveChangesAsync(ct);
            return Result<GetColorById.Response>.Success(
                new GetColorById.Response(entity.Id, entity.Name, entity.HexCode, entity.IsActive));
        }
    }
}
