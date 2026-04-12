using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Pos;

// ── GetWarehouses ──────────────────────────────────────────────────────────────
public static class GetWarehouses
{
    public sealed record Request(int PageIndex = 0, int PageSize = 10, string? Search = null);

    public sealed record Response(
        Guid Id, string Code, string Name, string SiteType,
        string? ManagerName, string? AddressLine1, string? City,
        string? Phone, string? Email, bool IsActive, bool IsDefault);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Request request, CancellationToken ct)
        {
            var query = _context.Warehouses
                .AsNoTracking()
                .Where(w => !w.IsDeleted);

            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(w => w.Name.Contains(request.Search) || w.Code.Contains(request.Search));

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(w => w.Code)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(w => new Response(
                    w.Id, w.Code, w.Name, w.SiteType,
                    w.ManagerName, w.AddressLine1, w.City,
                    w.Phone, w.Email, w.IsActive, w.IsDefault))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(
                new PagedResult<Response>(items, totalCount, request.PageIndex, request.PageSize));
        }
    }
}

// ── GetWarehouseById ───────────────────────────────────────────────────────────
public static class GetWarehouseById
{
    public sealed record Query(Guid Id);

    public sealed record PosCounterInfo(Guid Id, string CounterCode, string CounterName, bool IsActive);

    public sealed record Response(
        Guid Id, string Code, string Name, string SiteType,
        Guid? ParentId, string? ContactPerson, string? ManagerName,
        string? AddressLine1, string? AddressLine2, string? City, string? Area,
        string? State, string? PostalCode, string Country,
        string? Phone, string? Email,
        decimal? Latitude, decimal? Longitude,
        TimeOnly? OpeningTime, TimeOnly? ClosingTime,
        string? TaxNumber, bool IsDefault, bool IsActive,
        List<PosCounterInfo> Counters);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var entity = await _context.Warehouses
                .AsNoTracking()
                .Where(w => w.Id == query.Id && !w.IsDeleted)
                .Select(w => new Response(
                    w.Id, w.Code, w.Name, w.SiteType,
                    w.ParentId, w.ContactPerson, w.ManagerName,
                    w.AddressLine1, w.AddressLine2, w.City, w.Area,
                    w.State, w.PostalCode, w.Country,
                    w.Phone, w.Email,
                    w.Latitude, w.Longitude,
                    w.OpeningTime, w.ClosingTime,
                    w.TaxNumber, w.IsDefault, w.IsActive,
                    w.PosCounters
                        .Where(c => !c.IsDeleted)
                        .OrderBy(c => c.CounterCode)
                        .Select(c => new PosCounterInfo(c.Id, c.CounterCode, c.CounterName, c.IsActive))
                        .ToList()))
                .FirstOrDefaultAsync(ct);

            if (entity == null)
                return Result<Response>.Failure(Error.NotFound("Warehouse not found."));

            return Result<Response>.Success(entity);
        }
    }
}

// ── CreateWarehouse ────────────────────────────────────────────────────────────
public static class CreateWarehouse
{
    public sealed record Request(
        string Code, string Name, string SiteType,
        Guid? ParentId, string? ContactPerson, string? ManagerName,
        string? AddressLine1, string? AddressLine2, string? City, string? Area,
        string? State, string? PostalCode, string Country,
        string? Phone, string? Email,
        decimal? Latitude, decimal? Longitude,
        TimeOnly? OpeningTime, TimeOnly? ClosingTime,
        string? TaxNumber, bool IsDefault, bool IsActive);

    public sealed record Response(Guid Id, string Code, string Name);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Code).NotEmpty().MaximumLength(30);
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
            RuleFor(x => x.SiteType).NotEmpty().MaximumLength(30);
            RuleFor(x => x.Country).NotEmpty().MaximumLength(80);
            RuleFor(x => x.Email).MaximumLength(150);
            RuleFor(x => x.Phone).MaximumLength(30);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Request request, CancellationToken ct)
        {
            var codeExists = await _context.Warehouses
                .AnyAsync(w => w.Code == request.Code && !w.IsDeleted, ct);
            if (codeExists)
                return Result<Response>.Failure(
                    Error.Conflict($"Warehouse code '{request.Code}' already exists."));

            var entity = new Warehouses
            {
                Id = Guid.NewGuid(),
                Code = request.Code,
                Name = request.Name,
                SiteType = request.SiteType,
                ParentId = request.ParentId,
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
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                OpeningTime = request.OpeningTime,
                ClosingTime = request.ClosingTime,
                TaxNumber = request.TaxNumber,
                IsDefault = request.IsDefault,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.Warehouses.Add(entity);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(entity.Id, entity.Code, entity.Name));
        }
    }
}

// ── UpdateWarehouse ────────────────────────────────────────────────────────────
public static class UpdateWarehouse
{
    public sealed record Request(
        string Code, string Name, string SiteType,
        Guid? ParentId, string? ContactPerson, string? ManagerName,
        string? AddressLine1, string? AddressLine2, string? City, string? Area,
        string? State, string? PostalCode, string Country,
        string? Phone, string? Email,
        decimal? Latitude, decimal? Longitude,
        TimeOnly? OpeningTime, TimeOnly? ClosingTime,
        string? TaxNumber, bool IsDefault, bool IsActive);

    public sealed record Command(
        Guid Id, string Code, string Name, string SiteType,
        Guid? ParentId, string? ContactPerson, string? ManagerName,
        string? AddressLine1, string? AddressLine2, string? City, string? Area,
        string? State, string? PostalCode, string Country,
        string? Phone, string? Email,
        decimal? Latitude, decimal? Longitude,
        TimeOnly? OpeningTime, TimeOnly? ClosingTime,
        string? TaxNumber, bool IsDefault, bool IsActive);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Code).NotEmpty().MaximumLength(30);
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
            RuleFor(x => x.SiteType).NotEmpty().MaximumLength(30);
            RuleFor(x => x.Country).NotEmpty().MaximumLength(80);
            RuleFor(x => x.Email).MaximumLength(150);
            RuleFor(x => x.Phone).MaximumLength(30);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<GetWarehouseById.Response>> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.Warehouses
                .FirstOrDefaultAsync(w => w.Id == command.Id && !w.IsDeleted, ct);

            if (entity == null)
                return Result<GetWarehouseById.Response>.Failure(Error.NotFound("Warehouse not found."));

            if (entity.Code != command.Code)
            {
                var codeExists = await _context.Warehouses
                    .AnyAsync(w => w.Code == command.Code && w.Id != command.Id && !w.IsDeleted, ct);
                if (codeExists)
                    return Result<GetWarehouseById.Response>.Failure(
                        Error.Conflict($"Warehouse code '{command.Code}' already exists."));
            }

            entity.Code = command.Code;
            entity.Name = command.Name;
            entity.SiteType = command.SiteType;
            entity.ParentId = command.ParentId;
            entity.ContactPerson = command.ContactPerson;
            entity.ManagerName = command.ManagerName;
            entity.AddressLine1 = command.AddressLine1;
            entity.AddressLine2 = command.AddressLine2;
            entity.City = command.City;
            entity.Area = command.Area;
            entity.State = command.State;
            entity.PostalCode = command.PostalCode;
            entity.Country = command.Country;
            entity.Phone = command.Phone;
            entity.Email = command.Email;
            entity.Latitude = command.Latitude;
            entity.Longitude = command.Longitude;
            entity.OpeningTime = command.OpeningTime;
            entity.ClosingTime = command.ClosingTime;
            entity.TaxNumber = command.TaxNumber;
            entity.IsDefault = command.IsDefault;
            entity.IsActive = command.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            var counters = await _context.PosCounters
                .AsNoTracking()
                .Where(c => c.WarehouseId == entity.Id && !c.IsDeleted)
                .OrderBy(c => c.CounterCode)
                .Select(c => new GetWarehouseById.PosCounterInfo(c.Id, c.CounterCode, c.CounterName, c.IsActive))
                .ToListAsync(ct);

            return Result<GetWarehouseById.Response>.Success(
                new GetWarehouseById.Response(
                    entity.Id, entity.Code, entity.Name, entity.SiteType,
                    entity.ParentId, entity.ContactPerson, entity.ManagerName,
                    entity.AddressLine1, entity.AddressLine2, entity.City, entity.Area,
                    entity.State, entity.PostalCode, entity.Country,
                    entity.Phone, entity.Email,
                    entity.Latitude, entity.Longitude,
                    entity.OpeningTime, entity.ClosingTime,
                    entity.TaxNumber, entity.IsDefault, entity.IsActive, counters));
        }
    }
}

// ── DeleteWarehouse ────────────────────────────────────────────────────────────
public static class DeleteWarehouse
{
    public sealed record Command(Guid Id);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.Warehouses
                .FirstOrDefaultAsync(w => w.Id == command.Id && !w.IsDeleted, ct);

            if (entity == null)
                return Result.Failure(Error.NotFound("Warehouse not found."));

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}

// ── ToggleWarehouseActive ──────────────────────────────────────────────────────
public static class ToggleWarehouseActive
{
    public sealed record Command(Guid Id);

    public sealed record Response(Guid Id, bool IsActive);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.Warehouses
                .FirstOrDefaultAsync(w => w.Id == command.Id && !w.IsDeleted, ct);

            if (entity == null)
                return Result<Response>.Failure(Error.NotFound("Warehouse not found."));

            entity.IsActive = !entity.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(entity.Id, entity.IsActive));
        }
    }
}

// ── GetWarehouseStats ──────────────────────────────────────────────────────────
public static class GetWarehouseStats
{
    public sealed record Query();

    public sealed record Response(int TotalCount, int ActiveCount);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var total = await _context.Warehouses
                .CountAsync(w => !w.IsDeleted, ct);
            var active = await _context.Warehouses
                .CountAsync(w => !w.IsDeleted && w.IsActive, ct);

            return Result<Response>.Success(new Response(total, active));
        }
    }
}
