using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;

namespace EcommercePos.Api.Endpoints;

public static class WarehouseEndpoints
{
    public static void MapWarehouseEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/warehouses").WithTags("Warehouses");

        group.MapGet("/", async (
            [AsParameters] GetWarehousesRequest request,
            ApplicationDbContext context,
            CancellationToken ct) =>
        {
            var query = context.Warehouses
                .Where(w => !w.IsDeleted)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(w => w.Name.Contains(request.Search) || w.Code.Contains(request.Search));
            }

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(w => w.Name)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(w => new WarehouseResponse(
                    w.Id, w.Code, w.Name, w.SiteType, w.ManagerName,
                    w.AddressLine1, w.City, w.Phone, w.Email, w.IsActive))
                .ToListAsync(ct);

            return Results.Ok(new { data = items, totalCount });
        })
        .WithName("GetWarehouses")
        .WithSummary("Get paginated warehouses");

        group.MapGet("/{code}", async (string code, ApplicationDbContext context, CancellationToken ct) =>
        {
            var warehouse = await context.Warehouses
                .Where(w => w.Code == code && !w.IsDeleted)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (warehouse == null)
                return Results.NotFound(new { error = "Warehouse not found" });

            return Results.Ok(new { data = new WarehouseResponse(
                warehouse.Id, warehouse.Code, warehouse.Name, warehouse.SiteType, warehouse.ManagerName,
                warehouse.AddressLine1, warehouse.City, warehouse.Phone, warehouse.Email, warehouse.IsActive) });
        })
        .WithName("GetWarehouseByCode")
        .WithSummary("Get warehouse by code");

        group.MapPost("/", async (CreateWarehouseRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var exists = await context.Warehouses.AnyAsync(w => w.Code == request.Code, ct);
            if (exists)
                return Results.Conflict(new { error = "Warehouse code already exists" });

            var warehouse = new Warehouses
            {
                Id = Guid.NewGuid(),
                Code = request.Code,
                Name = request.Name,
                SiteType = request.SiteType,
                ContactPerson = request.ContactPerson,
                ManagerName = request.ManagerName,
                AddressLine1 = request.AddressLine1,
                AddressLine2 = request.AddressLine2,
                City = request.City,
                Area = request.Area,
                State = request.State,
                PostalCode = request.PostalCode,
                Country = request.Country,
                Phone = request.Phone,
                Email = request.Email,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            context.Warehouses.Add(warehouse);
            await context.SaveChangesAsync(ct);

            return Results.Created($"/api/warehouses/{warehouse.Code}", new { data = new WarehouseResponse(
                warehouse.Id, warehouse.Code, warehouse.Name, warehouse.SiteType, warehouse.ManagerName,
                warehouse.AddressLine1, warehouse.City, warehouse.Phone, warehouse.Email, warehouse.IsActive) });
        })
        .WithName("CreateWarehouse")
        .WithSummary("Create a new warehouse");

        group.MapPut("/{code}", async (string code, UpdateWarehouseRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var warehouse = await context.Warehouses.FirstOrDefaultAsync(w => w.Code == code, ct);
            if (warehouse == null || warehouse.IsDeleted)
                return Results.NotFound(new { error = "Warehouse not found" });

            if (warehouse.Code != request.Code)
            {
                var exists = await context.Warehouses.AnyAsync(w => w.Code == request.Code, ct);
                if (exists)
                    return Results.Conflict(new { error = "Warehouse code already exists" });
            }

            warehouse.Code = request.Code;
            warehouse.Name = request.Name;
            warehouse.SiteType = request.SiteType;
            warehouse.ContactPerson = request.ContactPerson;
            warehouse.ManagerName = request.ManagerName;
            warehouse.AddressLine1 = request.AddressLine1;
            warehouse.AddressLine2 = request.AddressLine2;
            warehouse.City = request.City;
            warehouse.Area = request.Area;
            warehouse.State = request.State;
            warehouse.PostalCode = request.PostalCode;
            warehouse.Country = request.Country;
            warehouse.Phone = request.Phone;
            warehouse.Email = request.Email;
            warehouse.IsActive = request.IsActive;
            warehouse.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(ct);
            return Results.Ok(new { data = new WarehouseResponse(
                warehouse.Id, warehouse.Code, warehouse.Name, warehouse.SiteType, warehouse.ManagerName,
                warehouse.AddressLine1, warehouse.City, warehouse.Phone, warehouse.Email, warehouse.IsActive) });
        })
        .WithName("UpdateWarehouse")
        .WithSummary("Update an existing warehouse");

        group.MapDelete("/{code}", async (string code, ApplicationDbContext context, CancellationToken ct) =>
        {
            var warehouse = await context.Warehouses.FirstOrDefaultAsync(w => w.Code == code, ct);
            if (warehouse == null || warehouse.IsDeleted)
                return Results.NotFound(new { error = "Warehouse not found" });

            warehouse.IsDeleted = true;
            warehouse.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(ct);
            return Results.NoContent();
        })
        .WithName("DeleteWarehouse")
        .WithSummary("Soft delete a warehouse");
    }
}

public record GetWarehousesRequest(int PageIndex = 0, int PageSize = 10, string? Search = null);
public record WarehouseResponse(
    Guid Id, string Code, string Name, string SiteType, string? ManagerName,
    string? AddressLine1, string? City, string? Phone, string? Email, bool IsActive);
public record CreateWarehouseRequest(
    string Code, string Name, string SiteType, string? ContactPerson, string? ManagerName,
    string? AddressLine1, string? AddressLine2, string? City, string? Area, string? State,
    string? PostalCode, string Country, string? Phone, string? Email, bool IsActive);
public record UpdateWarehouseRequest(
    string Code, string Name, string SiteType, string? ContactPerson, string? ManagerName,
    string? AddressLine1, string? AddressLine2, string? City, string? Area, string? State,
    string? PostalCode, string Country, string? Phone, string? Email, bool IsActive);
