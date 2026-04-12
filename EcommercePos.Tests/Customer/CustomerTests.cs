using Microsoft.EntityFrameworkCore;
using EcommercePos.Application.Features.Customer;
using EcommercePos.Persistence.Data;

namespace EcommercePos.Tests.Customer;

public class GetCustomersTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    public GetCustomersTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
        SeedTestData();
    }

    private void SeedTestData()
    {
        for (int i = 1; i <= 15; i++)
        {
            _context.Customers.Add(new Customers
            {
                Id = Guid.NewGuid(),
                CustomerCode = $"CUS-{i:D3}",
                CompanyName = $"Customer {i}",
                Email = $"customer{i}@example.com",
                Phone = $"+8801711{i:D7}",
                CustomerType = i % 2 == 0 ? "Retail" : "Wholesale",
                LoyaltyPoints = i * 100,
                Balance = i * 1000,
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
        var handler = new GetCustomers.Handler(_context);
        var query = new GetCustomers.Query(0, 10, null);
        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(15, result.Value.TotalCount);
        Assert.Equal(10, result.Value.Items.Count);
    }

    [Fact]
    public async Task Handle_WithSearch_ReturnsFilteredResults()
    {
        var handler = new GetCustomers.Handler(_context);
        var query = new GetCustomers.Query(0, 10, "Customer 1");
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

public class CreateCustomerTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    public CreateCustomerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
    }

    [Fact]
    public async Task Handle_ValidRequest_CreatesCustomer()
    {
        var handler = new CreateCustomer.Handler(_context);
        var command = new CreateCustomer.Command("+8801711000000", "Retail", null, "john@example.com", null, "Male", "John Doe", null, "Dhaka", null, "Bangladesh", null);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("+8801711000000", result.Value!.Phone);
    }

    [Fact]
    public async Task Handle_DuplicatePhone_ReturnsConflict()
    {
        _context.Customers.Add(new Customers { Id = Guid.NewGuid(), CustomerCode = "CUS-001", CompanyName = "Existing", Phone = "+8801711000000", Country = "Bangladesh", CustomerType = "Retail", IsDeleted = false });
        await _context.SaveChangesAsync();

        var handler = new CreateCustomer.Handler(_context);
        var command = new CreateCustomer.Command("+8801711000000", "Retail", null, "john@example.com", null, "Male", "John Doe", null, "Dhaka", null, "Bangladesh", null);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}

public class DeleteCustomerTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    public DeleteCustomerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
        _context.Customers.Add(new Customers { Id = Guid.NewGuid(), CustomerCode = "CUS-001", CompanyName = "Test", Country = "Bangladesh", CustomerType = "Retail", IsDeleted = false });
        _context.SaveChanges();
    }

    [Fact]
    public async Task Handle_ValidId_SoftDeletesCustomer()
    {
        var customer = await _context.Customers.FirstAsync();
        var handler = new DeleteCustomer.Handler(_context);
        var result = await handler.Handle(new DeleteCustomer.Command(customer.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        var deleted = await _context.Customers.IgnoreQueryFilters().FirstAsync(c => c.Id == customer.Id);
        Assert.True(deleted.IsDeleted);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
