using System.Security.Claims;
using Backend.Controllers;
using Backend.Data;
using Backend.DTOs.request;
using Backend.DTOs.response;
using Backend.Entities;
using Backend.Entities.Enums;
using Backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace BackendTests.Integration.Controllers;

public class UsersControllerTests : IDisposable
{
    private readonly DataContext _dataContext;
    private readonly UserService _userService;
    private readonly UsersController _controller;

    /// <summary>
    /// Assigns the context to the UsersControllerTests class
    /// </summary>
    public UsersControllerTests()
    {
        // Initialize in-memory database
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dataContext = new DataContext(options);

        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(c => c["Jwt:Key"]).Returns("SuperSecretKeyForTestingThatIsLongEnough");
        mockConfig.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
        mockConfig.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");

        _userService = new UserService(mockConfig.Object, _dataContext);
        _controller = new UsersController(_userService);
    }

    /// <summary>
    /// Dispose method to delete the database after the tests are run
    /// </summary>
    public void Dispose()
    {
        // Clean up the in-memory database
        _dataContext.Database.EnsureDeleted();
        _dataContext.Dispose();
    }

    private void SeedData()
    {
        var user = new Tutor
        {
            Id = 1,
            Name = "João Silva",
            Email = "joao@example.com",
            PhoneNumber = "912345678",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
            Address = "Rua do João",
            ZipCode = "1234-567",
            Role = UserRole.Tutor
        };

        _dataContext.Tutors.Add((Tutor)user);
        _dataContext.SaveChanges();
        _dataContext.ChangeTracker.Clear();
    }

    [Fact]
    public async Task Login_ReturnsOk_WhenCredentialsValid()
    {
        // Arrange
        SeedData();
        var loginRequest = new LoginRequestDto
        {
            Email = "joao@example.com",
            Password = "password"
        };

        var httpContext = new DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

        // Act
        var result = await _controller.Login(loginRequest);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ApiResponse<LoginResponseDto>>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<LoginResponseDto>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal("Autenticação efetuada", response.Message);
        Assert.NotNull(response.Data);
        Assert.Equal("João Silva", response.Data.Name);
        Assert.Equal(UserRole.Tutor, response.Data.Role);

        // Check if cookie is set
        var cookies = httpContext.Response.Headers["Set-Cookie"];
        Assert.Contains("access_token=", Assert.Single(cookies));
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_WhenInvalidEmail()
    {
        // Arrange
        var loginRequest = new LoginRequestDto
        {
            Email = "invalid@example.com",
            Password = "password"
        };

        // Act
        var result = await _controller.Login(loginRequest);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ApiResponse<LoginResponseDto>>>(result);
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<LoginResponseDto>>(unauthorizedResult.Value);
        Assert.False(response.Success);
        Assert.Equal("Email ou password inválido", response.Message);
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_WhenInvalidPassword()
    {
        // Arrange
        SeedData();
        var loginRequest = new LoginRequestDto
        {
            Email = "joao@example.com",
            Password = "wrongpassword"
        };

        // Act
        var result = await _controller.Login(loginRequest);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ApiResponse<LoginResponseDto>>>(result);
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<LoginResponseDto>>(unauthorizedResult.Value);
        Assert.False(response.Success);
        Assert.Equal("Email ou password inválido", response.Message);
    }

    [Fact]
    public async Task Login_ReturnsBadRequest_WhenModelStateInvalid()
    {
        // Arrange
        var loginRequest = new LoginRequestDto
        {
            Email = "", // Invalid
            Password = "password123"
        };

        _controller.ModelState.AddModelError("Email", "Email is required");

        // Act
        var result = await _controller.Login(loginRequest);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ApiResponse<LoginResponseDto>>>(result);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<LoginResponseDto>>(badRequestResult.Value);
        Assert.False(response.Success);
        Assert.Equal("Erros de validação", response.Message);
    }
}
