using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Category;

public static class ToggleCategoryActive
{
    public sealed record Command(Guid CategoryId);
    public sealed record Response(Guid Id, bool IsActive);
    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;
        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var category = await _context.Categories.FindAsync(new object[] { command.CategoryId }, ct);
            if (category == null || category.IsDeleted)
                return Result<Response>.Failure(Error.NotFound("Category not found"));
            category.IsActive = !category.IsActive;
            category.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);
            return Result<Response>.Success(new Response(category.Id, category.IsActive));
        }
    }
}