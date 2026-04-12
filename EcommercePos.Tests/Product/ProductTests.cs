using Microsoft.EntityFrameworkCore;
using EcommercePos.Application.Features.Product.Commands;
using EcommercePos.Application.Features.Product.Queries;
using EcommercePos.Persistence.Data;

namespace EcommercePos.Tests.Product;

public class GetProductsTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    public GetProductsTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
        SeedTestData();
    }

    private void SeedTestData()
    {
        var category = new Categories { Id = Guid.NewGuid(), Name = "Electronics", Slug = "electronics", IsActive = true, IsDeleted = false };
        var brand = new Brands { Id = Guid.NewGuid(), BrandCode = "BR-001", Name = "Apple", Slug = "apple", IsActive = true, IsDeleted = false };
        
        _context.Categories.Add(category);
        _context.Brands.Add(brand);
        
        for (int i = 1; i <= 15; i++)
        {
            _context.Products.Add(new Products
            {
                Id = Guid.NewGuid(),
                ProductCode = $"PRD-{i:D3}",
                Name = $"Product {i}",
                Sku = $"SKU-{i:D3}",
                CategoryId = category.Id,
                BrandId = brand.Id,
                CostPrice = 100 + i * 10,
                SalePrice = 150 + i * 15,
                ProductType = "Standard",
                Slug = $"product-{i}",
                IsActive = i % 2 == 0,
                IsDeleted = false
            });
        }
        _context.SaveChanges();
    }

    [Fact]
    public async Task Handle_DefaultRequest_ReturnsFirstPage()
    {
        var handler = new GetProducts.Handler(_context);
        var query = new GetProducts.Query(0, 10, null, null, null);
        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(15, result.Value.TotalCount);
        Assert.Equal(10, result.Value.Items.Count);
    }

    [Fact]
    public async Task Handle_WithSearch_ReturnsFilteredResults()
    {
        var handler = new GetProducts.Handler(_context);
        var query = new GetProducts.Query(0, 10, "Product 1", null, null);
        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(7, result.Value.TotalCount);
    }

    [Fact]
    public async Task Handle_WithCategoryFilter_ReturnsFilteredResults()
    {
        var category = await _context.Categories.FirstAsync();
        var handler = new GetProducts.Handler(_context);
        var query = new GetProducts.Query(0, 10, null, category.Id, null);
        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(15, result.Value.TotalCount);
    }

    [Fact]
    public async Task Handle_Pagination_WorksCorrectly()
    {
        var handler = new GetProducts.Handler(_context);
        var query = new GetProducts.Query(1, 5, null, null, null);
        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(5, result.Value.Items.Count);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}

public class CreateProductTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    public CreateProductTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
    }

    [Fact]
    public async Task Handle_ValidRequest_CreatesProduct()
    {
        var category = new Categories { Id = Guid.NewGuid(), Name = "Electronics", Slug = "electronics", IsActive = true, IsDeleted = false };
        var brand = new Brands { Id = Guid.NewGuid(), BrandCode = "BR-001", Name = "Apple", Slug = "apple", IsActive = true, IsDeleted = false };
        _context.Categories.Add(category);
        _context.Brands.Add(brand);
        await _context.SaveChangesAsync();

        var handler = new CreateProduct.Handler(_context);
        var command = new CreateProduct.Command("PRD-001", "Test Product", null, null, "Standard", 100, 150, null, false, false, true, category.Id, brand.Id, null, "SKU-001", null);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Test Product", result.Value!.Name);
    }

    [Fact]
    public async Task Handle_DuplicateProductCode_ReturnsConflict()
    {
        var category = new Categories { Id = Guid.NewGuid(), Name = "Electronics", Slug = "electronics", IsActive = true, IsDeleted = false };
        _context.Categories.Add(category);
        _context.Products.Add(new Products { Id = Guid.NewGuid(), ProductCode = "PRD-001", Name = "Existing", CategoryId = category.Id, ProductType = "Standard", Slug = "existing", IsDeleted = false });
        await _context.SaveChangesAsync();

        var handler = new CreateProduct.Handler(_context);
        var command = new CreateProduct.Command("PRD-001", "Test Product", null, null, "Standard", 100, 150, null, false, false, true, category.Id, null, null, "SKU-001", null);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}

public class DeleteProductTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    public DeleteProductTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
        SeedTestData();
    }

    private void SeedTestData()
    {
        var category = new Categories { Id = Guid.NewGuid(), Name = "Electronics", Slug = "electronics", IsActive = true, IsDeleted = false };
        _context.Categories.Add(category);
        _context.Products.Add(new Products { Id = Guid.NewGuid(), ProductCode = "PRD-001", Name = "Test Product", CategoryId = category.Id, ProductType = "Standard", Slug = "test-product", IsDeleted = false });
        _context.SaveChanges();
    }

    [Fact]
    public async Task Handle_ValidId_SoftDeletesProduct()
    {
        var product = await _context.Products.FirstAsync();
        var handler = new DeleteProduct.Handler(_context);
        var command = new DeleteProduct.Command(product.Id);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        
        var deleted = await _context.Products.IgnoreQueryFilters().FirstAsync(p => p.Id == product.Id);
        Assert.True(deleted.IsDeleted);
    }

    [Fact]
    public async Task Handle_InvalidId_ReturnsNotFound()
    {
        var handler = new DeleteProduct.Handler(_context);
        var command = new DeleteProduct.Command(Guid.NewGuid());
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
