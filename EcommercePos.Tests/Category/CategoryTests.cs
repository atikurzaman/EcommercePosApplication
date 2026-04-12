using Microsoft.EntityFrameworkCore;
using EcommercePos.Application.Features.Category;
using EcommercePos.Persistence.Data;

namespace EcommercePos.Tests.Category;

public class GetCategoriesTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    public GetCategoriesTests()
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
            _context.Categories.Add(new Categories
            {
                Id = Guid.NewGuid(),
                Name = $"Category {i}",
                Slug = $"category-{i}",
                Description = $"Description {i}",
                DisplayOrder = i,
                IsActive = i % 2 == 0,
                IsDeleted = false
            });
        }
        _context.SaveChanges();
    }

    [Fact]
    public async Task Handle_DefaultRequest_ReturnsFirstPage()
    {
        var handler = new GetCategories.Handler(_context);
        var query = new GetCategories.Query(0, 10, null);
        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(10, result.Value.TotalCount);
    }

    [Fact]
    public async Task Handle_WithSearch_ReturnsFilteredResults()
    {
        var handler = new GetCategories.Handler(_context);
        var query = new GetCategories.Query(0, 10, "Category 1");
        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}

public class CreateCategoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    public CreateCategoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
    }

    [Fact]
    public async Task Handle_ValidRequest_CreatesCategory()
    {
        var handler = new CreateCategory.Handler(_context);
        var command = new CreateCategory.Command("Electronics", null, "Electronic products", null, null, 1, false, true, null, null);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Electronics", result.Value!.Name);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}

public class DeleteCategoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    public DeleteCategoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
        _context.Categories.Add(new Categories { Id = Guid.NewGuid(), Name = "Test", Slug = "test", IsDeleted = false });
        _context.SaveChanges();
    }

    [Fact]
    public async Task Handle_ValidId_SoftDeletesCategory()
    {
        var category = await _context.Categories.FirstAsync();
        var handler = new DeleteCategory.Handler(_context);
        var result = await handler.Handle(new DeleteCategory.Command(category.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
