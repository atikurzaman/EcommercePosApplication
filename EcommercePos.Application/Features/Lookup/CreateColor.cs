using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Lookup;

public static class CreateColor
{
    public sealed record Request(string Name, string? HexCode, bool IsActive);
    public sealed record Response(Guid Id, string Name);

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

        public async Task<Result<Response>> Handle(Request request, CancellationToken ct)
        {
            var entity = new Colors
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                HexCode = request.HexCode,
                IsActive = request.IsActive,
                IsDeleted = false
            };

            _context.Colors.Add(entity);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(entity.Id, entity.Name));
        }
    }
}
