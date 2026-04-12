using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Category;

public static class CreateCategory
{
    public sealed record Command(
        string Name, string? Slug, string? Description, string? ImageUrl,
        Guid? ParentCategoryId, int DisplayOrder, bool IsFeatured, bool IsActive,
        string? MetaTitle, string? MetaDescription);

    public sealed record Response(
        Guid Id, string Name, string? Slug, string? Description, string? ImageUrl,
        Guid? ParentCategoryId, int DisplayOrder, bool IsFeatured, bool IsActive,
        string? MetaTitle, string? MetaDescription);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Slug).MaximumLength(200);
            RuleFor(x => x.MetaTitle).MaximumLength(200);
            RuleFor(x => x.MetaDescription).MaximumLength(500);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var slug = command.Slug ?? command.Name.ToLower().Replace(" ", "-").Replace("--", "-");

            var item = new Categories
            {
                Id = Guid.NewGuid(),
                Name = command.Name,
                Slug = slug,
                Description = command.Description,
                ImageUrl = command.ImageUrl,
                ParentCategoryId = command.ParentCategoryId,
                DisplayOrder = command.DisplayOrder,
                IsFeatured = command.IsFeatured,
                IsActive = command.IsActive,
                MetaTitle = command.MetaTitle,
                MetaDescription = command.MetaDescription,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.Categories.Add(item);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(
                item.Id, item.Name, item.Slug, item.Description, item.ImageUrl,
                item.ParentCategoryId, item.DisplayOrder, item.IsFeatured, item.IsActive,
                item.MetaTitle, item.MetaDescription));
        }
    }
}
