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

public class TutorsControllerTests : IDisposable
{
    private readonly DataContext _dataContext;
    private readonly TutorService _tutorService;
    private readonly TutorsController _controller;

    /// <summary>
    /// Assigns the context to the TutorsControllerTests class
    /// </summary>
    public TutorsControllerTests()
    {
        // Initialize in-memory database
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dataContext = new DataContext(options);

        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(c => c["Jwt:Key"]).Returns("SuperSecretKeyForTestingPurposesNowLongEnoughToMeetThe256BitRequirementForHS256Algorithm");
        var userService = new UserService(mockConfig.Object, _dataContext);

        // Seed data
        var tutor1 = new Tutor
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

        var tutor2 = new Tutor
        {
            Id = 2,
            Name = "Pedro Costa",
            Email = "pedro@example.com",
            PhoneNumber = "912345679",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
            Address = "Rua do Pedro",
            ZipCode = "1234-568",
            Role = UserRole.Tutor
        };

        _dataContext.Tutors.AddRange(tutor1, tutor2);
        _dataContext.SaveChanges();

        _tutorService = new TutorService(_dataContext, userService);
        _controller = new TutorsController(_tutorService);
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

    /// <summary>
    /// Test that verifies if the `GetAllTutors` method returns an OK result with a list of tutors
    /// </summary>
    [Fact]
    public async Task GetAllTutors_ReturnsOkResult_WithListOfTutors()
    {
        // Act
        var result = await _controller.GetAllTutors();

        // Assert
        Assert.IsType<ActionResult<ApiResponse<List<TutorResponseDto>>>>(result);
    }

    /// <summary>
    /// Test that verifies if the `GetTutorById` method returns an OK result with a tutor
    /// </summary>
    [Fact]
    public async Task GetTutorById_ReturnsOkResult_WithTutor()
    {
        var tutorId = 1;

        // Act
        var result = await _controller.GetTutorById(tutorId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<TutorResponseDto>>(okResult.Value);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(1, response.Data.Id);
    }

    /// <summary>
    /// Test that verifies if the `GetTutorById` method returns a NotFound result when the tutor is not found
    /// </summary>
    [Fact]
    public async Task GetTutorById_ReturnsNotFound_WhenTutorNotFound()
    {
        var tutorId = 999;

        // Act
        var result = await _controller.GetTutorById(tutorId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<TutorResponseDto>>(notFoundResult.Value);
        Assert.False(response.Success);
        Assert.Equal("Não existe nenhum tutor com esse id", response.Message);
    }

    /// <summary>
    /// Test that verifies if the `CreateTutor` method returns a CreatedAtActionResult when the tutor is added
    /// </summary>
    [Fact]
    public async Task CreateTutor_ReturnsCreatedAtActionResult_WhenTutorAdded()
    {
        var newTutor = new CreateTutorRequestDto
        {
            Name = "Ana Pereira",
            Email = "ana@example.com",
            PhoneNumber = "912345681",
            Password = "password123",
            Address = "Rua da Ana",
            ZipCode = "1234-570"
        };

        // Act
        var result = await _controller.CreateTutor(newTutor);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var response = Assert.IsType<ApiResponse<CreateTutorResponseDto>>(createdAtActionResult.Value);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(newTutor.Name, response.Data.Name);
    }

    /// <summary>
    /// Test that verifies if the `CreateTutor` method returns a BadRequest when model state is invalid
    /// </summary>
    [Fact]
    public async Task CreateTutor_ReturnsBadRequest_WhenModelStateInvalid()
    {
        var newTutor = new CreateTutorRequestDto
        {
            Name = "",
            Email = "ana@example.com",
            PhoneNumber = "912345681",
            Password = "password123",
            Address = "Rua da Ana",
            ZipCode = "1234-570"
        };

        _controller.ModelState.AddModelError("Name", "Name is required");

        // Act
        var result = await _controller.CreateTutor(newTutor);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<CreateTutorResponseDto>>(badRequestResult.Value);
        Assert.False(response.Success);
        Assert.Equal("Erros de validação", response.Message);
    }

    /// <summary>
    /// Test that verifies if the `UpdateTutorById` method returns an OkObjectResult when the update is successful
    /// </summary>
    [Fact]
    public async Task UpdateTutorById_ReturnsOkResult_WhenUpdateSucceeds()
    {
        var tutorId = 1;
        var updatedTutor = new UpdateTutorRequestDto
        {
            Name = "Updated João",
            Address = "Updated Address",
            ZipCode = "9876-543"
        };

        // Act
        var result = await _controller.UpdateTutorById(tutorId, updatedTutor);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal("Os dados do tutor foram atualizados", response.Message);
    }

    /// <summary>
    /// Test that verifies if the `UpdateTutorById` method returns a BadRequest when model state is invalid
    /// </summary>
    [Fact]
    public async Task UpdateTutorById_ReturnsBadRequest_WhenModelStateInvalid()
    {
        var tutorId = 1;
        var updatedTutor = new UpdateTutorRequestDto
        {
            Name = "",
            Address = "Updated Address",
            ZipCode = "9876-543"
        };

        _controller.ModelState.AddModelError("Name", "Name is required");

        // Act
        var result = await _controller.UpdateTutorById(tutorId, updatedTutor);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(badRequestResult.Value);
        Assert.False(response.Success);
        Assert.Equal("Erros de validação", response.Message);
    }

    /// <summary>
    /// Test that verifies if the `UpdateTutorById` method returns a NotFound when the tutor does not exist
    /// </summary>
    [Fact]
    public async Task UpdateTutorById_ReturnsNotFound_WhenTutorNotFound()
    {
        var tutorId = 999;
        var updatedTutor = new UpdateTutorRequestDto
        {
            Name = "Updated João",
            Address = "Updated Address",
            ZipCode = "9876-543"
        };

        // Act
        var result = await _controller.UpdateTutorById(tutorId, updatedTutor);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(notFoundResult.Value);
        Assert.False(response.Success);
        Assert.Equal("Não existe nenhum tutor com esse id", response.Message);
    }

    /// <summary>
    /// Test that verifies if the `DeleteTutorById` method returns an OkObjectResult when the deletion is successful
    /// </summary>
    [Fact]
    public async Task DeleteTutorById_ReturnsOkResult_WhenDeletionSucceeds()
    {
        var tutorId = 1;

        // Act
        var result = await _controller.DeleteTutorById(tutorId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal("Tutor removido", response.Message);
    }

    /// <summary>
    /// Test that verifies if the `DeleteTutorById` method returns a NotFound when the tutor does not exist
    /// </summary>
    [Fact]
    public async Task DeleteTutorById_ReturnsNotFound_WhenTutorNotFound()
    {
        var tutorId = 999;

        // Act
        var result = await _controller.DeleteTutorById(tutorId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(notFoundResult.Value);
        Assert.False(response.Success);
        Assert.Equal("Não existe nenhum tutor com esse id", response.Message);
    }

}
