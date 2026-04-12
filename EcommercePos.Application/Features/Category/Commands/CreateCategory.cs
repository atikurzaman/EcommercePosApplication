using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Category.Commands;

public static class CreateCategory
{
    public sealed record Request
    {
        public string Name { get; init; } = string.Empty;
        public string? Slug { get; init; }
        public string? Description { get; init; }
        public string? ImageUrl { get; init; }
        public Guid? ParentCategoryId { get; init; }
        public int DisplayOrder { get; init; }
        public bool IsFeatured { get; init; }
        public bool IsActive { get; init; }
        public string? MetaTitle { get; init; }
        public string? MetaDescription { get; init; }
    }

    public sealed record Response
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? Slug { get; init; }
        public string? Description { get; init; }
        public string? ImageUrl { get; init; }
        public Guid? ParentCategoryId { get; init; }
        public int DisplayOrder { get; init; }
        public bool IsFeatured { get; init; }
        public bool IsActive { get; init; }
        public string? MetaTitle { get; init; }
        public string? MetaDescription { get; init; }
    }

    public sealed record Command(
        string Name, string? Slug, string? Description, string? ImageUrl, 
        Guid? ParentCategoryId, int DisplayOrder, bool IsFeatured, bool IsActive,
        string? MetaTitle, string? MetaDescription);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator() {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Slug).MaximumLength(200);
            RuleFor(x => x.MetaTitle).MaximumLength(200);
            RuleFor(x => x.MetaDescription).MaximumLength(500);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

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

            return Result<Response>.Success(item.Adapt<Response>());
        }
    }
}