using Microsoft.EntityFrameworkCore;
using EcommercePos.Application.Features.Brand.Commands;
using EcommercePos.Application.Features.Brand.Queries;
using EcommercePos.Persistence.Data;

namespace EcommercePos.Tests.Brand;

public class GetBrandsTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    public GetBrandsTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
        SeedTestData();
    }

    private void SeedTestData()
    {
        for (int i = 1; i <= 10; i++)
        {
            _context.Brands.Add(new Brands
            {
                Id = Guid.NewGuid(),
                BrandCode = $"BR-{i:D3}",
                Name = $"Brand {i}",
                Slug = $"brand-{i}",
                Description = $"Description {i}",
                IsActive = i % 2 == 0,
                IsDeleted = false
            });
        }
        _context.SaveChanges();
    }

    [Fact]
    public async Task Handle_DefaultRequest_ReturnsFirstPage()
    {
        var handler = new GetBrands.Handler(_context);
        var query = new GetBrands.Query(0, 10, null);
        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(10, result.Value.TotalCount);
    }

    [Fact]
    public async Task Handle_WithSearch_ReturnsFilteredResults()
    {
        var handler = new GetBrands.Handler(_context);
        var query = new GetBrands.Query(0, 10, "Brand 1");
        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}

public class CreateBrandTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    public CreateBrandTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
    }

    [Fact]
    public async Task Handle_ValidRequest_CreatesBrand()
    {
        var handler = new CreateBrand.Handler(_context);
        var command = new CreateBrand.Command("BR-001", "Apple", "Apple products", null, null, null, false, true);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}

public class DeleteBrandTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    public DeleteBrandTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
        _context.Brands.Add(new Brands { Id = Guid.NewGuid(), BrandCode = "BR-001", Name = "Test", Slug = "test", IsDeleted = false });
        _context.SaveChanges();
    }

    [Fact]
    public async Task Handle_ValidId_SoftDeletesBrand()
    {
        var brand = await _context.Brands.FirstAsync();
        var handler = new DeleteBrand.Handler(_context);
        var result = await handler.Handle(new DeleteBrand.Command(brand.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
