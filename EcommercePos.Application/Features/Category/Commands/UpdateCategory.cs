using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Category.Commands;

public static class UpdateCategory
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
        Guid Id, string Name, string? Slug, string? Description, string? ImageUrl, 
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
            var item = await _context.Categories
                .Where(x => x.Id == command.Id && !x.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (item == null)
            {
                return Result<Response>.Failure(Error.NotFound($"Category with id '{command.Id}' was not found."));
            }

            item.Name = command.Name;
            item.Slug = command.Slug ?? command.Name.ToLower().Replace(" ", "-").Replace("--", "-");
            item.Description = command.Description;
            item.ImageUrl = command.ImageUrl;
            item.ParentCategoryId = command.ParentCategoryId;
            item.DisplayOrder = command.DisplayOrder;
            item.IsFeatured = command.IsFeatured;
            item.IsActive = command.IsActive;
            item.MetaTitle = command.MetaTitle;
            item.MetaDescription = command.MetaDescription;
            item.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(item.Adapt<Response>());
        }
    }
}