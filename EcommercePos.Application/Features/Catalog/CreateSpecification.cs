using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Catalog;

public static class CreateSpecification
{
    public sealed record Request
    {
        public string SpecName { get; init; } = string.Empty;
        public int SortOrder { get; init; }
    }

    public sealed record Response
    {
        public Guid Id { get; init; }
        public string SpecName { get; init; } = string.Empty;
        public int SortOrder { get; init; }
    }

    public sealed record Command(string SpecName, int SortOrder);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.SpecName).NotEmpty().MaximumLength(200);
        }
    }

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var item = new ProductSpecifications
            {
                Id = Guid.NewGuid(),
                SpecName = command.SpecName,
                SortOrder = command.SortOrder,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.ProductSpecifications.Add(item);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response
            {
                Id = item.Id,
                SpecName = item.SpecName,
                SortOrder = item.SortOrder
            });
        }
    }
}
