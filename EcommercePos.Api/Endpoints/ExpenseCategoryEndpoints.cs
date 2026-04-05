using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;

namespace EcommercePos.Api.Endpoints;

public static class ExpenseCategoryEndpoints
{
    public static void MapExpenseCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/expense-categories").WithTags("ExpenseCategories");

        group.MapGet("/", async (
            [AsParameters] GetExpenseCategoriesRequest request,
            ApplicationDbContext context,
            CancellationToken ct) =>
        {
            var query = context.ExpenseCategories
                .Where(c => !c.IsDeleted)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(c => c.Name.Contains(request.Search));
            }

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(c => c.Name)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new ExpenseCategoryResponse(
                    c.Id, c.Name, c.Description, c.IsActive))
                .ToListAsync(ct);

            return Results.Ok(new { data = items, totalCount });
        })
        .WithName("GetExpenseCategories")
        .WithSummary("Get paginated expense categories");

        group.MapGet("/{id:guid}", async (Guid id, ApplicationDbContext context, CancellationToken ct) =>
        {
            var category = await context.ExpenseCategories
                .Where(c => c.Id == id && !c.IsDeleted)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (category == null)
                return Results.NotFound(new { error = "Expense category not found" });

            return Results.Ok(new { data = new ExpenseCategoryResponse(
                category.Id, category.Name, category.Description, category.IsActive) });
        })
        .WithName("GetExpenseCategoryById")
        .WithSummary("Get expense category by id");

        group.MapPost("/", async (CreateExpenseCategoryRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var category = new ExpenseCategories
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            context.ExpenseCategories.Add(category);
            await context.SaveChangesAsync(ct);

            return Results.Created($"/api/expense-categories/{category.Id}", new { data = new ExpenseCategoryResponse(
                category.Id, category.Name, category.Description, category.IsActive) });
        })
        .WithName("CreateExpenseCategory")
        .WithSummary("Create a new expense category");

        group.MapPut("/{id:guid}", async (Guid id, UpdateExpenseCategoryRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var category = await context.ExpenseCategories.FindAsync(new object[] { id }, ct);
            if (category == null || category.IsDeleted)
                return Results.NotFound(new { error = "Expense category not found" });

            category.Name = request.Name;
            category.Description = request.Description;
            category.IsActive = request.IsActive;
            category.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(ct);
            return Results.Ok(new { data = new ExpenseCategoryResponse(
                category.Id, category.Name, category.Description, category.IsActive) });
        })
        .WithName("UpdateExpenseCategory")
        .WithSummary("Update an existing expense category");

        group.MapDelete("/{id:guid}", async (Guid id, ApplicationDbContext context, CancellationToken ct) =>
        {
            var category = await context.ExpenseCategories.FindAsync(new object[] { id }, ct);
            if (category == null || category.IsDeleted)
                return Results.NotFound(new { error = "Expense category not found" });

            category.IsDeleted = true;
            category.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(ct);
            return Results.NoContent();
        })
        .WithName("DeleteExpenseCategory")
        .WithSummary("Soft delete an expense category");
    }
}

public record GetExpenseCategoriesRequest(int PageIndex = 0, int PageSize = 10, string? Search = null);
public record ExpenseCategoryResponse(
    Guid Id, string Name, string? Description, bool IsActive);
public record CreateExpenseCategoryRequest(
    string Name, string? Description, bool IsActive);
public record UpdateExpenseCategoryRequest(
    string Name, string? Description, bool IsActive);
