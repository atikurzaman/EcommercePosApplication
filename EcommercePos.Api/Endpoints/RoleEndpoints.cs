using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;

namespace EcommercePos.Api.Endpoints;

public static class RoleEndpoints
{
    public static void MapRoleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/roles").WithTags("Roles");

        group.MapGet("/", async (
            [AsParameters] GetRolesRequest request,
            ApplicationDbContext context,
            CancellationToken ct) =>
        {
            var query = context.Roles.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(r => r.Name.Contains(request.Search));
            }

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(r => r.Name)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(r => new RoleResponse(r.Id, r.Name, r.Description, r.IsActive))
                .ToListAsync(ct);

            return Results.Ok(new { data = items, totalCount });
        })
        .WithName("GetRoles")
        .WithSummary("Get paginated roles");

        group.MapGet("/{id:guid}", async (Guid id, ApplicationDbContext context, CancellationToken ct) =>
        {
            var role = await context.Roles
                .Where(r => r.Id == id)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (role == null)
                return Results.NotFound(new { error = "Role not found" });

            return Results.Ok(new { data = new RoleResponse(role.Id, role.Name, role.Description, role.IsActive) });
        })
        .WithName("GetRoleById")
        .WithSummary("Get role by id");

        group.MapPost("/", async (CreateRoleRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var role = new Roles
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                IsActive = request.IsActive
            };

            context.Roles.Add(role);
            await context.SaveChangesAsync(ct);

            return Results.Created($"/api/roles/{role.Id}", new { data = new RoleResponse(
                role.Id, role.Name, role.Description, role.IsActive) });
        })
        .WithName("CreateRole")
        .WithSummary("Create a new role");

        group.MapPut("/{id:guid}", async (Guid id, UpdateRoleRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var role = await context.Roles.FindAsync(new object[] { id }, ct);
            if (role == null)
                return Results.NotFound(new { error = "Role not found" });

            role.Name = request.Name;
            role.Description = request.Description;
            role.IsActive = request.IsActive;

            await context.SaveChangesAsync(ct);
            return Results.Ok(new { data = new RoleResponse(
                role.Id, role.Name, role.Description, role.IsActive) });
        })
        .WithName("UpdateRole")
        .WithSummary("Update an existing role");

        group.MapDelete("/{id:guid}", async (Guid id, ApplicationDbContext context, CancellationToken ct) =>
        {
            var role = await context.Roles.FindAsync(new object[] { id }, ct);
            if (role == null)
                return Results.NotFound(new { error = "Role not found" });

            context.Roles.Remove(role);
            await context.SaveChangesAsync(ct);
            return Results.NoContent();
        })
        .WithName("DeleteRole")
        .WithSummary("Delete a role");
    }
}

public record GetRolesRequest(int PageIndex = 0, int PageSize = 10, string? Search = null);
public record RoleResponse(Guid Id, string? Name, string? Description, bool IsActive);
public record CreateRoleRequest(string Name, string? Description, bool IsActive);
public record UpdateRoleRequest(string Name, string? Description, bool IsActive);
