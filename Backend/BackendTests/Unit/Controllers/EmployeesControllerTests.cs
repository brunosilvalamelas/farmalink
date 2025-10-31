using Backend.Controllers;
using Backend.DTOs.request;
using Backend.DTOs.response;
using Backend.Entities;
using Backend.Entities.Enums;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BackendTests.Unit.Controllers;

public class EmployeesControllerTests
{
    private readonly Mock<IEmployeeService> _mockEmployeeService;
    private readonly EmployeesController _controller;

    public EmployeesControllerTests()
    {
        _mockEmployeeService = new Mock<IEmployeeService>();
        _controller = new EmployeesController(_mockEmployeeService.Object);
    }

    [Fact]
    public async Task CreateEmployee_ReturnsCreatedAtAction_WhenValid()
    {
        // Arrange
        var dto = new CreateEmployeeRequestDto
        {
            Name = "Alice Santos",
            Email = "alice@example.com",
            PhoneNumber = "912345678",
            Password = "Password#123",
            DeliveryLocation = "Porto",
        };

        var created = new Employee
        {
            Id = 1,
            Name = dto.Name,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            DeliveryLocation = dto.DeliveryLocation,
            Role = UserRole.Employee
        };

        _mockEmployeeService
            .Setup(s => s.CreateEmployeeAsync(dto))
            .ReturnsAsync((created, "token"));

        // Act
        var result = await _controller.CreateEmployee(dto);

        // Assert
        var action = Assert.IsType<ActionResult<ApiResponse<CreateEmployeeResponseDto>>>(result);
        var createdAt = Assert.IsType<CreatedAtActionResult>(action.Result);
        var response = Assert.IsType<ApiResponse<CreateEmployeeResponseDto>>(createdAt.Value);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(dto.Name, response.Data!.Name);
        Assert.Equal(UserRole.Employee, response.Data.Role);
    }

    [Fact]
    public async Task CreateEmployee_ReturnsBadRequest_WhenModelStateInvalid()
    {
        // Arrange
        var dto = new CreateEmployeeRequestDto
        {
            Name = "", // inválido
            Email = "alice@example.com",
            PhoneNumber = "912345678",
            Password = "Password#123",
            DeliveryLocation = "Porto",
        };
        _controller.ModelState.AddModelError("Name", "Name is required");

        // Act
        var result = await _controller.CreateEmployee(dto);

        // Assert
        var action = Assert.IsType<ActionResult<ApiResponse<CreateEmployeeResponseDto>>>(result);
        var bad = Assert.IsType<BadRequestObjectResult>(action.Result);
        var response = Assert.IsType<ApiResponse<CreateEmployeeResponseDto>>(bad.Value);
        Assert.False(response.Success);
        Assert.Equal("Erros de validação", response.Message);
    }

    [Fact]
    public async Task GetAllEmployees_ReturnsOkWithList()
    {
        // Arrange
        var list = new List<Employee> {
            new Employee { Id = 1, Name = "Alice", Email = "alice@example.com", PhoneNumber = "912345678", DeliveryLocation = "Porto" }
        };

        _mockEmployeeService
            .Setup(s => s.GetAllEmployeesAsync())
            .ReturnsAsync(list);

        // Act
        var result = await _controller.GetAllEmployees();

        // Assert
        var action = Assert.IsType<ActionResult<ApiResponse<List<EmployeeResponseDto>>>>(result);
        var ok = Assert.IsType<OkObjectResult>(action.Result);
        var response = Assert.IsType<ApiResponse<List<EmployeeResponseDto>>>(ok.Value);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Single(response.Data!);
        Assert.Equal("Employees encontrados.", response.Message);
    }

    [Fact]
    public async Task GetEmployeeById_ReturnsOk_WhenFound()
    {
        // Arrange
        var emp = new Employee { Id = 1, Name = "Alice", Email = "alice@example.com", PhoneNumber = "912345678", DeliveryLocation = "Porto" };

        _mockEmployeeService
            .Setup(s => s.GetEmployeeByIdAsync(1))
            .ReturnsAsync(emp);

        // Act
        var result = await _controller.GetEmployeeById(1);

        // Assert
        var action = Assert.IsType<ActionResult<ApiResponse<EmployeeResponseDto>>>(result);
        var ok = Assert.IsType<OkObjectResult>(action.Result);
        var response = Assert.IsType<ApiResponse<EmployeeResponseDto>>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal(1, response.Data!.Id);
    }

    [Fact]
    public async Task GetEmployeeById_ReturnsNotFound_WhenMissing()
    {
        // Arrange
        _mockEmployeeService
            .Setup(s => s.GetEmployeeByIdAsync(999))
            .ReturnsAsync((Employee?)null);

        // Act
        var result = await _controller.GetEmployeeById(999);

        // Assert
        var action = Assert.IsType<ActionResult<ApiResponse<EmployeeResponseDto>>>(result);
        var nf = Assert.IsType<NotFoundObjectResult>(action.Result);
        var response = Assert.IsType<ApiResponse<EmployeeResponseDto>>(nf.Value);
        Assert.False(response.Success);
        Assert.Equal("Não existe nenhum employee com esse id", response.Message);
    }

    [Fact]
    public async Task UpdateEmployeeById_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var dto = new UpdateEmployeeRequestDto
        {
            Name = "Alice Atualizada",
            DeliveryLocation = "Coimbra",
        };

        _mockEmployeeService
            .Setup(s => s.UpdateEmployeeAsync(1, dto))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.UpdateEmployeeById(1, dto);

        // Assert
        var action = Assert.IsType<ActionResult<ApiResponse<bool>>>(result);
        var ok = Assert.IsType<OkObjectResult>(action.Result);
        var response = Assert.IsType<ApiResponse<bool>>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal("Os dados do employee foram atualizados", response.Message);
    }

    [Fact]
    public async Task UpdateEmployeeById_ReturnsBadRequest_WhenModelStateInvalid()
    {
        // Arrange
        var dto = new UpdateEmployeeRequestDto
        {
            Name = "", // inválido
            DeliveryLocation = "Coimbra",
        };
        _controller.ModelState.AddModelError("Name", "Name is required");

        // Act
        var result = await _controller.UpdateEmployeeById(1, dto);

        // Assert
        var action = Assert.IsType<ActionResult<ApiResponse<bool>>>(result);
        var bad = Assert.IsType<BadRequestObjectResult>(action.Result);
        var response = Assert.IsType<ApiResponse<bool>>(bad.Value);
        Assert.False(response.Success);
        Assert.Equal("Erros de validação", response.Message);
    }

    [Fact]
    public async Task UpdateEmployeeById_ReturnsNotFound_WhenMissing()
    {
        // Arrange
        var dto = new UpdateEmployeeRequestDto
        {
            Name = "Ghost",
            DeliveryLocation = "Aveiro",
        };

        _mockEmployeeService
            .Setup(s => s.UpdateEmployeeAsync(999, dto))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.UpdateEmployeeById(999, dto);

        // Assert
        var action = Assert.IsType<ActionResult<ApiResponse<bool>>>(result);
        var nf = Assert.IsType<NotFoundObjectResult>(action.Result);
        var response = Assert.IsType<ApiResponse<bool>>(nf.Value);
        Assert.False(response.Success);
        Assert.Equal("Não existe nenhum employee com esse id", response.Message);
    }

    [Fact]
    public async Task DeleteEmployeeById_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        _mockEmployeeService
            .Setup(s => s.DeleteEmployeeAsync(1))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteEmployeeById(1);

        // Assert
        var action = Assert.IsType<ActionResult<ApiResponse<bool>>>(result);
        var ok = Assert.IsType<OkObjectResult>(action.Result);
        var response = Assert.IsType<ApiResponse<bool>>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal("Employee removido", response.Message);
    }

    [Fact]
    public async Task DeleteEmployeeById_ReturnsNotFound_WhenMissing()
    {
        // Arrange
        _mockEmployeeService
            .Setup(s => s.DeleteEmployeeAsync(999))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteEmployeeById(999);

        // Assert
        var action = Assert.IsType<ActionResult<ApiResponse<bool>>>(result);
        var nf = Assert.IsType<NotFoundObjectResult>(action.Result);
        var response = Assert.IsType<ApiResponse<bool>>(nf.Value);
        Assert.False(response.Success);
        Assert.Equal("Não existe nenhum employee com esse id", response.Message);
    }
}
