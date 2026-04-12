using Microsoft.EntityFrameworkCore;
using EcommercePos.Application.Features.Supplier;
using EcommercePos.Persistence.Data;

namespace EcommercePos.Tests.Supplier;

public class GetSuppliersTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    public GetSuppliersTests()
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
            _context.Suppliers.Add(new Suppliers
            {
                Id = Guid.NewGuid(),
                SupplierCode = $"SUP-{i:D3}",
                Name = $"Supplier {i}",
                ContactPerson = $"Contact {i}",
                Email = $"supplier{i}@example.com",
                Phone = $"+8801711{i:D7}",
                City = "Dhaka",
                Country = "Bangladesh",
                IsActive = true,
                IsDeleted = false
            });
        }
        _context.SaveChanges();
    }

    [Fact]
    public async Task Handle_DefaultRequest_ReturnsFirstPage()
    {
        var handler = new GetSuppliers.Handler(_context);
        var query = new GetSuppliers.Query(0, 10, null);
        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(10, result.Value.TotalCount);
    }

    [Fact]
    public async Task Handle_WithSearch_ReturnsFilteredResults()
    {
        var handler = new GetSuppliers.Handler(_context);
        var query = new GetSuppliers.Query(0, 10, "Supplier 1");
        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}

public class CreateSupplierTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    public CreateSupplierTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
    }

    [Fact]
    public async Task Handle_ValidRequest_CreatesSupplier()
    {
        var handler = new CreateSupplier.Handler(_context);
        var command = new CreateSupplier.Command("ABC Corp", "+8801711000000", null, "John Doe", null, "john@abc.com", "Dhaka", null, null, null, null, "Bangladesh", null, "TAX123", null, null, null);
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

public class DeleteSupplierTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    public DeleteSupplierTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
        _context.Suppliers.Add(new Suppliers { Id = Guid.NewGuid(), SupplierCode = "SUP-001", Name = "Test", Country = "Bangladesh", IsDeleted = false });
        _context.SaveChanges();
    }

    [Fact]
    public async Task Handle_ValidId_SoftDeletesSupplier()
    {
        var supplier = await _context.Suppliers.FirstAsync();
        var handler = new DeleteSupplier.Handler(_context);
        var result = await handler.Handle(new DeleteSupplier.Command(supplier.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
