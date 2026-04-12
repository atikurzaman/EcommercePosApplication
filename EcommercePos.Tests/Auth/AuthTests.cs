using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using EcommercePos.Application.Features.Auth;
using EcommercePos.Persistence.Data;

namespace EcommercePos.Tests.Auth;

public class LoginUserTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly LoginUser.Handler _handler;

    public LoginUserTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt:Key", "ThisIsASecretKeyForJwtTokenGeneration123" },
                { "Jwt:Issuer", "EcommercePos.Api" },
                { "Jwt:Audience", "EcommercePos.AdminPortal" },
                { "Jwt:ExpiryInMinutes", "60" }
            })
            .Build();

        _handler = new LoginUser.Handler(_context, _configuration);

        SeedTestData();
    }

    private void SeedTestData()
    {
        var role = new Roles { Id = Guid.NewGuid(), Name = "Admin", IsActive = true };
        _context.Roles.Add(role);

        var user = new Users
        {
            Id = Guid.NewGuid(),
            UserName = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            IsActive = true,
            PasswordHash = HashPassword("password123"),
            EmailConfirmed = true
        };
        _context.Users.Add(user);

        var userRole = new UserRoles { UserId = user.Id, RoleId = role.Id };
        _context.UserRoles.Add(userRole);

        _context.SaveChanges();
    }

    private static string HashPassword(string password)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsSuccessWithToken()
    {
        var command = new LoginUser.Command("test@example.com", "password123");
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.NotEmpty(result.Value.AccessToken);
        Assert.NotEmpty(result.Value.RefreshToken);
    }

    [Fact]
    public async Task Handle_InvalidEmail_ReturnsUnauthorized()
    {
        var command = new LoginUser.Command("wrong@example.com", "password123");
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid credentials", result.Error?.Message);
    }

    [Fact]
    public async Task Handle_InvalidPassword_ReturnsUnauthorized()
    {
        var command = new LoginUser.Command("test@example.com", "wrongpassword");
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid credentials", result.Error?.Message);
    }

    [Fact]
    public async Task Handle_InactiveUser_ReturnsUnauthorized()
    {
        var user = await _context.Users.FirstAsync();
        user.IsActive = false;
        await _context.SaveChangesAsync();

        var command = new LoginUser.Command("test@example.com", "password123");
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Account is disabled", result.Error?.Message);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}

public class RegisterUserTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly RegisterUser.Handler _handler;

    public RegisterUserTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
        _handler = new RegisterUser.Handler(_context);

        SeedTestData();
    }

    private void SeedTestData()
    {
        var role = new Roles { Id = Guid.NewGuid(), Name = "Admin", IsActive = true, Description = "Administrator role" };
        _context.Roles.Add(role);
        _context.SaveChanges();
    }

    [Fact]
    public async Task Handle_ValidRequest_CreatesUser()
    {
        var command = new RegisterUser.Command("newuser@example.com", "password123", "New", "User", null);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("newuser@example.com", result.Value.Email);
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ReturnsConflict()
    {
        var command = new RegisterUser.Command("existing@example.com", "password123", "Existing", "User", null);
        _context.Users.Add(new Users { Id = Guid.NewGuid(), Email = "existing@example.com", UserName = "existing", PasswordHash = "hash", IsActive = true });
        await _context.SaveChangesAsync();

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Email already registered", result.Error?.Message);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
