using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Catalog;

public static class CreateTag
{
    public sealed record Request
    {
        public string Name { get; init; } = string.Empty;
        public string? Slug { get; init; }
    }

    public sealed record Response
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Slug { get; init; } = string.Empty;
    }

    public sealed record Command(string Name, string? Slug);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Slug).MaximumLength(200);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var slug = command.Slug ?? command.Name.ToLower().Replace(" ", "-").Replace("--", "-");

            var item = new Tags
            {
                Id = Guid.NewGuid(),
                Name = command.Name,
                Slug = slug,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.Tags.Add(item);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response
            {
                Id = item.Id,
                Name = item.Name,
                Slug = item.Slug
            });
        }
    }
}
