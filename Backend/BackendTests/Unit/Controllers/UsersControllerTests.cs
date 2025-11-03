using Backend.Controllers;
using Backend.DTOs.request;
using Backend.DTOs.response;
using Backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BackendTests.Unit.Controllers;

public class UsersControllerTests
{
    private readonly Mock<IUserService> _mockUserService;
    private readonly UsersController _controller;

    public UsersControllerTests()
    {
        _mockUserService = new Mock<IUserService>();
        _controller = new UsersController(_mockUserService.Object);
    }

    [Fact]
    public async Task Login_ReturnsOk_WhenCredentialsValid()
    {
        // Arrange
        var loginRequest = new LoginRequestDto
        {
            Email = "test@example.com",
            Password = "password123"
        };

        var loginResponse = new LoginResponseDto
        {
            Name = "Test User",
            Role = Backend.Entities.Enums.UserRole.Tutor
        };

        var authResponse = new AuthResponseDto(loginResponse, "fake-jwt-token");

        _mockUserService
            .Setup(s => s.AuthenticateUserAsync(loginRequest.Email, loginRequest.Password))
            .ReturnsAsync(authResponse);

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
        Assert.Equal(loginResponse.Name, response.Data.Name);
        Assert.Equal(loginResponse.Role, response.Data.Role);

        // Check if cookie is set
        var cookies = httpContext.Response.Headers["Set-Cookie"];
        Assert.Contains("access_token=fake-jwt-token", Assert.Single(cookies));
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_WhenInvalidCredentials()
    {
        // Arrange
        var loginRequest = new LoginRequestDto
        {
            Email = "invalid@example.com",
            Password = "wrongpassword"
        };

        _mockUserService
            .Setup(s => s.AuthenticateUserAsync(loginRequest.Email, loginRequest.Password))
            .ReturnsAsync((AuthResponseDto?)null);

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
