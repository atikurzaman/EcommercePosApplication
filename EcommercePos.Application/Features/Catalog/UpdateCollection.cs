using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Catalog;

public static class UpdateCollection
{
    public sealed record Command(
        Guid Id, string Name, string? Slug, string? Description, string? ImageUrl,
        int DisplayOrder, bool IsActive, bool ShowInHomePage);

    public sealed record Response(Guid Id, string Name, string Slug);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Slug).MaximumLength(200);
            RuleFor(x => x.Description).MaximumLength(2000);
            RuleFor(x => x.ImageUrl).MaximumLength(500);
        }
    }

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.ProductCollections
                .Where(c => c.Id == command.Id && !c.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (entity == null)
                return Result<Response>.Failure(Error.NotFound($"Collection with id '{command.Id}' was not found."));

            entity.Name = command.Name;
            entity.Slug = command.Slug ?? command.Name.ToLower().Replace(" ", "-").Replace("--", "-");
            entity.Description = command.Description;
            entity.ImageUrl = command.ImageUrl;
            entity.DisplayOrder = command.DisplayOrder;
            entity.IsActive = command.IsActive;
            entity.ShowInHomePage = command.ShowInHomePage;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);
            return Result<Response>.Success(new Response(entity.Id, entity.Name, entity.Slug));
        }
    }
}
