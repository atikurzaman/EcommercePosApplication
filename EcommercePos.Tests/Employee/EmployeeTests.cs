using Microsoft.EntityFrameworkCore;
using EcommercePos.Application.Features.Employee;
using EcommercePos.Persistence.Data;

namespace EcommercePos.Tests.Employee;

public class GetEmployeesTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    public GetEmployeesTests()
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
            _context.Employees.Add(new Employees
            {
                Id = Guid.NewGuid(),
                EmployeeCode = $"EMP-{i:D3}",
                FirstName = $"FirstName{i}",
                LastName = $"LastName{i}",
                Email = $"employee{i}@example.com",
                Phone = $"+8801711{i:D7}",
                Department = "IT",
                Designation = "Developer",
                IsActive = true,
                IsDeleted = false
            });
        }
        _context.SaveChanges();
    }

    [Fact]
    public async Task Handle_DefaultRequest_ReturnsFirstPage()
    {
        var handler = new GetEmployees.Handler(_context);
        var query = new GetEmployees.Query(0, 10, null);
        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(10, result.Value.TotalCount);
    }

    [Fact]
    public async Task Handle_WithSearch_ReturnsFilteredResults()
    {
        var handler = new GetEmployees.Handler(_context);
        var query = new GetEmployees.Query(0, 10, "FirstName1");
        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}

public class CreateEmployeeTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    public CreateEmployeeTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
    }

    [Fact]
    public async Task Handle_ValidRequest_CreatesEmployee()
    {
        var handler = new CreateEmployee.Handler(_context);
        var command = new CreateEmployee.Command("John", null, "Doe", null, null, "john@example.com", null, null, null, null, "Developer", "IT", null, 50000, null, null, null, null, null);
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

public class DeleteEmployeeTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    public DeleteEmployeeTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
        _context.Employees.Add(new Employees { Id = Guid.NewGuid(), EmployeeCode = "EMP-001", FirstName = "John", LastName = "Doe", IsDeleted = false });
        _context.SaveChanges();
    }

    [Fact]
    public async Task Handle_ValidId_SoftDeletesEmployee()
    {
        var employee = await _context.Employees.FirstAsync();
        var handler = new DeleteEmployee.Handler(_context);
        var result = await handler.Handle(new DeleteEmployee.Command(employee.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
