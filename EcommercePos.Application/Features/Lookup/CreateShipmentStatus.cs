using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Lookup;

public static class CreateShipmentStatus
{
    public sealed record Request(string StatusCode, string DisplayName, byte SortOrder);
    public sealed record Response(string StatusCode, string DisplayName);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.StatusCode).NotEmpty().MaximumLength(30);
            RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(80);
        }
    }

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Request request, CancellationToken ct)
        {
            var exists = await _context.ShipmentStatuses.AnyAsync(c => c.StatusCode == request.StatusCode, ct);
            if (exists)
                return Result<Response>.Failure(Error.Conflict($"Shipment status '{request.StatusCode}' already exists."));

            var entity = new ShipmentStatuses
            {
                StatusCode = request.StatusCode,
                DisplayName = request.DisplayName,
                SortOrder = request.SortOrder
            };

            _context.ShipmentStatuses.Add(entity);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(entity.StatusCode, entity.DisplayName));
        }
    }
}
