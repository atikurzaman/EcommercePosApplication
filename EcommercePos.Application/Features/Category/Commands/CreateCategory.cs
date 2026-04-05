using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;
using CategoryEntity = EcommercePos.Persistence.Data.Categories;

namespace EcommercePos.Application.Features.Category.Commands;

public static class CreateCategory
{
    public sealed record Request(string Name, string? Description, string? ImageUrl, Guid? ParentCategoryId, int DisplayOrder, bool IsActive, bool IsFeatured);

    public sealed record Command(string Name, string? Description, string? ImageUrl, Guid? ParentCategoryId, int DisplayOrder, bool IsActive, bool IsFeatured);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<CategoryEntity>> Handle(Command command, CancellationToken ct)
        {
            var category = new CategoryEntity
            {
                Id = Guid.NewGuid(),
                Name = command.Name,
                Description = command.Description,
                ImageUrl = command.ImageUrl,
                ParentCategoryId = command.ParentCategoryId,
                DisplayOrder = command.DisplayOrder,
                IsActive = command.IsActive,
                IsFeatured = command.IsFeatured,
                Slug = command.Name.ToLower().Replace(" ", "-"),
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync(ct);

            return Result<CategoryEntity>.Success(category);
        }
    }
}
