using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Catalog;

public static class CreateCollection
{
    public sealed record Request(
        string Name, string? Slug, string? Description, string? ImageUrl,
        int DisplayOrder, bool IsActive, bool ShowInHomePage);

    public sealed record Response(Guid Id, string Name, string Slug);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
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

        public async Task<Result<Response>> Handle(Request request, CancellationToken ct)
        {
            var slug = request.Slug ?? request.Name.ToLower().Replace(" ", "-").Replace("--", "-");

            var entity = new ProductCollections
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Slug = slug,
                Description = request.Description,
                ImageUrl = request.ImageUrl,
                DisplayOrder = request.DisplayOrder,
                IsActive = request.IsActive,
                ShowInHomePage = request.ShowInHomePage,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.ProductCollections.Add(entity);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(entity.Id, entity.Name, entity.Slug));
        }
    }
}
