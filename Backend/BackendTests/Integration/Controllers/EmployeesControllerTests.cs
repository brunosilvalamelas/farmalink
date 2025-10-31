using Backend.Controllers;
using Backend.Data;
using Backend.DTOs.request;
using Backend.DTOs.response;
using Backend.Entities;
using Backend.Entities.Enums;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace BackendTests.Integration.Controllers;

public class EmployeesControllerTests : IDisposable
{
    private readonly DataContext _dataContext;
    private readonly EmployeesController _controller;

    private readonly EmployeeService _employeeService;

    public EmployeesControllerTests()
    {
        // InMemory DB isolada por teste
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dataContext = new DataContext(options);

        // Mock de IConfiguration para o UserService (JWT)
        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(c => c["Jwt:Key"])
            .Returns("SuperSecretKeyForTestingPurposesNowLongEnoughToMeetThe256BitRequirementForHS256Algorithm");

        var userService = new UserService(mockConfig.Object, _dataContext);

        // Seed
        var emp1 = new Employee
        {
            Id = 1,
            Name = "Alice Santos",
            Email = "alice@example.com",
            PhoneNumber = "912345678",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
            DeliveryLocation = "Porto",
            Role = UserRole.Employee
        };
        var emp2 = new Employee
        {
            Id = 2,
            Name = "Bruno Costa",
            Email = "bruno@example.com",
            PhoneNumber = "912345679",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
            DeliveryLocation = "Lisboa",
            Role = UserRole.Employee
        };

        _dataContext.Employees.AddRange(emp1, emp2);
        _dataContext.SaveChanges();

        _employeeService = new EmployeeService(_dataContext, userService);
        _controller = new EmployeesController(_employeeService);
    }

    public void Dispose()
    {
        _dataContext.Database.EnsureDeleted();
        _dataContext.Dispose();
    }

    // GET /api/employees
    [Fact]
    public async Task GetAllEmployees_ReturnsOk_WithList()
    {
        var result = await _controller.GetAllEmployees();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<List<EmployeeResponseDto>>>(ok.Value);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.True(response.Data!.Count >= 2);
    }

    // GET /api/employees/{id} (ok)
    [Fact]
    public async Task GetEmployeeById_ReturnsOk_WhenExists()
    {
        var result = await _controller.GetEmployeeById(1);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<EmployeeResponseDto>>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal(1, response.Data!.Id);
        Assert.Equal("Alice Santos", response.Data!.Name);
    }

    // GET /api/employees/{id} (not found)
    [Fact]
    public async Task GetEmployeeById_ReturnsNotFound_WhenMissing()
    {
        var result = await _controller.GetEmployeeById(999);

        var nf = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<EmployeeResponseDto>>(nf.Value);
        Assert.False(response.Success);
        Assert.Equal("Não existe nenhum employee com esse id", response.Message);
    }

    // POST /api/employees (created)
    [Fact]
    public async Task CreateEmployee_ReturnsCreatedAt_WhenValid()
    {
        var dto = new CreateEmployeeRequestDto
        {
            Name = "Carla Lima",
            Email = "carla@example.com",
            PhoneNumber = "913222333",
            Password = "Password#123",
            DeliveryLocation = "Braga",
        };

        var result = await _controller.CreateEmployee(dto);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var response = Assert.IsType<ApiResponse<CreateEmployeeResponseDto>>(created.Value);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(dto.Name, response.Data!.Name);
        Assert.Equal(UserRole.Employee, response.Data!.Role);
    }

    // POST /api/employees (bad request – model state inválido)
    [Fact]
    public async Task CreateEmployee_ReturnsBadRequest_WhenModelStateInvalid()
    {
        var dto = new CreateEmployeeRequestDto
        {
            Name = "",                      
            Email = "carla@example.com",
            PhoneNumber = "913222333",
            Password = "Password#123",
            DeliveryLocation = "Braga",
        };

        _controller.ModelState.AddModelError("Name", "Name is required");

        var result = await _controller.CreateEmployee(dto);

        var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<CreateEmployeeResponseDto>>(bad.Value);
        Assert.False(response.Success);
        Assert.Equal("Erros de validação", response.Message);
    }

    // PUT /api/employees/{id} (ok)
    [Fact]
    public async Task UpdateEmployeeById_ReturnsOk_WhenSuccess()
    {
        var dto = new UpdateEmployeeRequestDto
        {
            Name = "Alice Atualizada",
            DeliveryLocation = "Coimbra",
        };

        var result = await _controller.UpdateEmployeeById(1, dto);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal("Os dados do employee foram atualizados", response.Message);
    }

    // PUT /api/employees/{id} (bad request)
    [Fact]
    public async Task UpdateEmployeeById_ReturnsBadRequest_WhenModelStateInvalid()
    {
        var dto = new UpdateEmployeeRequestDto
        {
            Name = "",
            DeliveryLocation = "Coimbra",
        };

        _controller.ModelState.AddModelError("Name", "Name is required");

        var result = await _controller.UpdateEmployeeById(1, dto);

        var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(bad.Value);
        Assert.False(response.Success);
        Assert.Equal("Erros de validação", response.Message);
    }

    // PUT /api/employees/{id} (not found)
    [Fact]
    public async Task UpdateEmployeeById_ReturnsNotFound_WhenMissing()
    {
        var dto = new UpdateEmployeeRequestDto
        {
            Name = "Nao Existe",
            DeliveryLocation = "Aveiro",
        };

        var result = await _controller.UpdateEmployeeById(999, dto);

        var nf = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(nf.Value);
        Assert.False(response.Success);
        Assert.Equal("Não existe nenhum employee com esse id", response.Message);
    }

    // DELETE /api/employees/{id} (ok)
    [Fact]
    public async Task DeleteEmployeeById_ReturnsOk_WhenSuccess()
    {
        var result = await _controller.DeleteEmployeeById(1);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal("Employee removido", response.Message);
    }

    // DELETE /api/employees/{id} (not found)
    [Fact]
    public async Task DeleteEmployeeById_ReturnsNotFound_WhenMissing()
    {
        var result = await _controller.DeleteEmployeeById(999);

        var nf = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(nf.Value);
        Assert.False(response.Success);
        Assert.Equal("Não existe nenhum employee com esse id", response.Message);
    }
}
