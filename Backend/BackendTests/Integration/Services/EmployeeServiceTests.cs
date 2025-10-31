using Backend.Data;
using Backend.DTOs.request;
using Backend.Entities;
using Backend.Entities.Enums;
using Backend.Exceptions;
using Backend.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace BackendTests.Integration.Services;

public class EmployeeServiceTests : IDisposable
{
    private readonly DataContext _dataContext;
    private readonly EmployeeService _employeeService;
    private readonly UserService _userService;

    private readonly int _emp1Id;
    private readonly int _emp2Id;

    public EmployeeServiceTests()
    {
        // DB InMemory única por suite
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dataContext = new DataContext(options);

        // Mock de IConfiguration para o UserService (JWT)
        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(c => c["Jwt:Key"])
            .Returns("SuperSecretKeyForTestingPurposesNowLongEnoughToMeetThe256BitRequirementForHS256Algorithm");

        _userService = new UserService(mockConfig.Object, _dataContext);

        // Seed Employees
        var e1 = new Employee
        {
            Name = "Alice Santos",
            Email = "alice@example.com",
            PhoneNumber = "912345678",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
            DeliveryLocation = "Porto",
            ZipCode = "4000-001",
            Role = UserRole.Employee
        };
        var e2 = new Employee
        {
            Name = "Bruno Costa",
            Email = "bruno@example.com",
            PhoneNumber = "912345679",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
            DeliveryLocation = "Lisboa",
            ZipCode = "1000-001",
            Role = UserRole.Employee
        };

        _dataContext.Employees.AddRange(e1, e2);
        _dataContext.SaveChanges();

        _emp1Id = e1.Id;
        _emp2Id = e2.Id;

        _employeeService = new EmployeeService(_dataContext, _userService);
    }

    public void Dispose()
    {
        _dataContext.Database.EnsureDeleted();
        _dataContext.Dispose();
    }

    [Fact]
    public async Task GetAllEmployeesAsync_ShouldReturnSeededTwo()
    {
        var list = await _employeeService.GetAllEmployeesAsync();

        Assert.NotNull(list);
        Assert.Equal(2, list.Count);
        Assert.Contains(list, e => e.Id == _emp1Id);
        Assert.Contains(list, e => e.Id == _emp2Id);
    }

    [Fact]
    public async Task GetEmployeeByIdAsync_ShouldReturnEntity_WhenExists()
    {
        var emp = await _employeeService.GetEmployeeByIdAsync(_emp1Id);

        Assert.NotNull(emp);
        Assert.Equal(_emp1Id, emp!.Id);
        Assert.Equal("Alice Santos", emp.Name);
    }

    [Fact]
    public async Task GetEmployeeByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        var emp = await _employeeService.GetEmployeeByIdAsync(999);
        Assert.Null(emp);
    }

    [Fact]
    public async Task CreateEmployeeAsync_ShouldCreate_AndReturnToken()
    {
        var dto = new CreateEmployeeRequestDto
        {
            Name = "Carla Lima",
            Email = "carla@example.com",
            PhoneNumber = "913222333",
            Password = "Password#123",
            DeliveryLocation = "Braga",
        };

        var (employee, token) = await _employeeService.CreateEmployeeAsync(dto);

        Assert.NotNull(employee);
        Assert.True(employee.Id > 0);
        Assert.Equal(dto.Name, employee.Name);
        Assert.Equal(UserRole.Employee, employee.Role);
        Assert.False(string.IsNullOrWhiteSpace(token));
    }

    [Fact]
    public async Task CreateEmployeeAsync_ShouldThrowValidation_WhenEmailDuplicate()
    {
        var dto = new CreateEmployeeRequestDto
        {
            Name = "Outra Pessoa",
            Email = "alice@example.com", // duplicado do seed
            PhoneNumber = "913000111",
            Password = "Password#123",
            DeliveryLocation = "Porto",
        };

        await Assert.ThrowsAsync<ValidationException>(async () =>
        {
            await _employeeService.CreateEmployeeAsync(dto);
        });
    }

    [Fact]
    public async Task UpdateEmployeeAsync_ShouldReturnTrue_WhenExists()
    {
        var dto = new UpdateEmployeeRequestDto
        {
            Name = "Alice Atualizada",
            DeliveryLocation = "Coimbra",
        };

        var ok = await _employeeService.UpdateEmployeeAsync(_emp1Id, dto);

        Assert.True(ok);

        // confirma persistência
        var updated = await _employeeService.GetEmployeeByIdAsync(_emp1Id);
        Assert.NotNull(updated);
        Assert.Equal("Alice Atualizada", updated!.Name);
        Assert.Equal("alice.updated@example.com", updated.Email);
        Assert.Equal("Coimbra", updated.DeliveryLocation);
    }

    [Fact]
    public async Task UpdateEmployeeAsync_ShouldReturnFalse_WhenMissing()
    {
        var dto = new UpdateEmployeeRequestDto
        {
            Name = "Ghost",
            DeliveryLocation = "Aveiro",
        };

        var ok = await _employeeService.UpdateEmployeeAsync(999, dto);
        Assert.False(ok);
    }

    [Fact]
    public async Task DeleteEmployeeAsync_ShouldReturnTrue_WhenExists()
    {
        var ok = await _employeeService.DeleteEmployeeAsync(_emp1Id);
        Assert.True(ok);

        var emp = await _employeeService.GetEmployeeByIdAsync(_emp1Id);
        Assert.Null(emp);
    }

    [Fact]
    public async Task DeleteEmployeeAsync_ShouldReturnFalse_WhenMissing()
    {
        var ok = await _employeeService.DeleteEmployeeAsync(999);
        Assert.False(ok);
    }
}
