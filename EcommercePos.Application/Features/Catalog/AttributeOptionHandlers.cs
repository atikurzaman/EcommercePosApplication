using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Catalog;

public static class GetAttributeOptions
{
    public sealed record Request(Guid AttributeTypeId);

    public sealed record Response(
        Guid Id, string Value, string? DisplayValue, Guid? ColorId, int SortOrder, bool IsActive);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<List<Response>>> Handle(Request request, CancellationToken ct)
        {
            var items = await _context.AttributeOptions
                .AsNoTracking()
                .Where(o => o.AttributeTypeId == request.AttributeTypeId && !o.IsDeleted)
                .OrderBy(o => o.SortOrder)
                .Select(o => new Response(o.Id, o.Value, o.DisplayValue, o.ColorId, o.SortOrder, o.IsActive))
                .ToListAsync(ct);

            return Result<List<Response>>.Success(items);
        }
    }
}

public static class CreateAttributeOption
{
    public sealed record Request(
        Guid AttributeTypeId, string Value, string? DisplayValue,
        Guid? ColorId, int SortOrder, bool IsActive);

    public sealed record Response(Guid Id, string Value);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.AttributeTypeId).NotEmpty();
            RuleFor(x => x.Value).NotEmpty().MaximumLength(200);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Request request, CancellationToken ct)
        {
            var typeExists = await _context.AttributeTypes
                .AnyAsync(a => a.Id == request.AttributeTypeId && !a.IsDeleted, ct);

            if (!typeExists)
                return Result<Response>.Failure(Error.NotFound("Attribute type not found."));

            var entity = new AttributeOptions
            {
                Id = Guid.NewGuid(),
                AttributeTypeId = request.AttributeTypeId,
                Value = request.Value,
                DisplayValue = request.DisplayValue,
                ColorId = request.ColorId,
                SortOrder = request.SortOrder,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.AttributeOptions.Add(entity);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(entity.Id, entity.Value));
        }
    }
}

public static class UpdateAttributeOption
{
    public sealed record Request(string Value, string? DisplayValue, Guid? ColorId, int SortOrder, bool IsActive);
    public sealed record Command(Guid Id, string Value, string? DisplayValue, Guid? ColorId, int SortOrder, bool IsActive);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Value).NotEmpty().MaximumLength(200);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<GetAttributeOptions.Response>> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.AttributeOptions
                .FirstOrDefaultAsync(o => o.Id == command.Id && !o.IsDeleted, ct);

            if (entity == null)
                return Result<GetAttributeOptions.Response>.Failure(Error.NotFound("Attribute option not found."));

            entity.Value = command.Value;
            entity.DisplayValue = command.DisplayValue;
            entity.ColorId = command.ColorId;
            entity.SortOrder = command.SortOrder;
            entity.IsActive = command.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            return Result<GetAttributeOptions.Response>.Success(
                new GetAttributeOptions.Response(
                    entity.Id, entity.Value, entity.DisplayValue,
                    entity.ColorId, entity.SortOrder, entity.IsActive));
        }
    }
}

public static class DeleteAttributeOption
{
    public sealed record Command(Guid Id);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.AttributeOptions
                .FirstOrDefaultAsync(o => o.Id == command.Id && !o.IsDeleted, ct);

            if (entity == null)
                return Result.Failure(Error.NotFound("Attribute option not found."));

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}

public static class BulkCreateAttributeOptions
{
    public sealed record OptionInput(string Value, string? DisplayValue, Guid? ColorId, int SortOrder, bool IsActive);

    public sealed record Request(Guid AttributeTypeId, List<OptionInput> Options);

    public sealed record Response(int CreatedCount);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.AttributeTypeId).NotEmpty();
            RuleFor(x => x.Options).NotEmpty();
            RuleForEach(x => x.Options).ChildRules(option =>
            {
                option.RuleFor(o => o.Value).NotEmpty().MaximumLength(200);
            });
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Request request, CancellationToken ct)
        {
            var typeExists = await _context.AttributeTypes
                .AnyAsync(a => a.Id == request.AttributeTypeId && !a.IsDeleted, ct);

            if (!typeExists)
                return Result<Response>.Failure(Error.NotFound("Attribute type not found."));

            foreach (var opt in request.Options)
            {
                _context.AttributeOptions.Add(new AttributeOptions
                {
                    Id = Guid.NewGuid(),
                    AttributeTypeId = request.AttributeTypeId,
                    Value = opt.Value,
                    DisplayValue = opt.DisplayValue,
                    ColorId = opt.ColorId,
                    SortOrder = opt.SortOrder,
                    IsActive = opt.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                });
            }

            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(request.Options.Count));
        }
    }
}
