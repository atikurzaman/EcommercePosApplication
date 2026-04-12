using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;
using Microsoft.Extensions.Configuration;
using FluentValidation;

namespace EcommercePos.Application.Features.Auth;

public static class GetRoles
{
    public sealed record Query;
    public sealed record Response(Guid Id, string Name, string? Description, bool IsActive);
    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) { _context = context; }
        public async Task<List<Response>> Handle(Query query, CancellationToken ct)
        {
            return await _context.Roles.Where(r => r.IsActive).Select(r => new Response(r.Id, r.Name!, r.Description, r.IsActive)).ToListAsync(ct);
        }
    }
}
