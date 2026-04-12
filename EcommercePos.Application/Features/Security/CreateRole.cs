using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Security;

public static class CreateRole
{
    public sealed record Request(string Name, string? Description, bool IsActive);
    public sealed record Response(Guid Id, string Name);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(256);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Request request, CancellationToken ct)
        {
            var exists = await _context.Roles
                .AnyAsync(r => r.Name == request.Name, ct);

            if (exists)
                return Result<Response>.Failure(Error.Conflict($"Role '{request.Name}' already exists."));

            var entity = new Roles
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                NormalizedName = request.Name.ToUpperInvariant(),
                Description = request.Description,
                IsActive = request.IsActive,
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            _context.Roles.Add(entity);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(entity.Id, entity.Name));
        }
    }
}
