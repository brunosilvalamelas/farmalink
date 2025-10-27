using Backend.Controllers;
using Backend.Data;
using Backend.DTOs.request;
using Backend.DTOs.response;
using Backend.Services;
using Backend.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace BackendTests.Integration.Controllers;

public class TutorsControllerTests : IDisposable
{
    private readonly DataContext _dataContext;
    private readonly TutorService _tutorService;
    private readonly PatientService _patientService;
    private readonly TutorsController _controller;

    /// <summary>
    /// Assigns the context to the TutorsControllerTests class
    /// </summary>
    public TutorsControllerTests()
    {
        // Initialize in-memory database
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "TestTutorDb")
            .Options;

        _dataContext = new DataContext(options);

        // Seed data
        var tutor1 = new Tutor
        {
            Id = 1,
            Name = "João Silva",
            Email = "joao@example.com",
            PhoneNumber = "912345678",
            Address = "Rua do João",
            ZipCode = "1234-567"
        };

        var tutor2 = new Tutor
        {
            Id = 2,
            Name = "Pedro Costa",
            Email = "pedro@example.com",
            PhoneNumber = "912345679",
            Address = "Rua do Pedro",
            ZipCode = "1234-568"
        };

        _dataContext.Tutors.AddRange(tutor1, tutor2);
        _dataContext.SaveChanges();

        _tutorService = new TutorService(_dataContext);
        _patientService = new PatientService(_dataContext);
        _controller = new TutorsController(_tutorService, _patientService);
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
        var newTutor = new TutorRequestDto
        {
            Name = "Ana Pereira",
            Email = "ana@example.com",
            PhoneNumber = "912345681",
            Address = "Rua da Ana",
            ZipCode = "1234-570"
        };

        // Act
        var result = await _controller.CreateTutor(newTutor);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var response = Assert.IsType<ApiResponse<TutorResponseDto>>(createdAtActionResult.Value);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(newTutor.Name, response.Data.Name);
        Assert.Equal(newTutor.Email, response.Data.Email);
    }

    /// <summary>
    /// Test that verifies if the `CreateTutor` method returns a BadRequest when model state is invalid
    /// </summary>
    [Fact]
    public async Task CreateTutor_ReturnsBadRequest_WhenModelStateInvalid()
    {
        var newTutor = new TutorRequestDto
        {
            Name = "",
            Email = "ana@example.com",
            PhoneNumber = "912345681",
            Address = "Rua da Ana",
            ZipCode = "1234-570"
        };

        _controller.ModelState.AddModelError("Name", "Name is required");

        // Act
        var result = await _controller.CreateTutor(newTutor);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<TutorResponseDto>>(badRequestResult.Value);
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
        var updatedTutor = new TutorRequestDto
        {
            Name = "Updated João",
            Email = "updated@example.com",
            PhoneNumber = "965432109",
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
        var updatedTutor = new TutorRequestDto
        {
            Name = "",
            Email = "updated@example.com",
            PhoneNumber = "965432109",
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
        var updatedTutor = new TutorRequestDto
        {
            Name = "Updated João",
            Email = "updated@example.com",
            PhoneNumber = "965432109",
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

    /// <summary>
    /// Test that verifies if the `RegisterPatient` method returns a CreatedAtActionResult when the patient is registered
    /// </summary>
    [Fact]
    public async Task RegisterPatient_ReturnsCreatedAtActionResult_WhenPatientRegistered()
    {
        var tutorId = 1;
        var newPatient = new PatientRequestDto
        {
            Name = "Ana Pereira",
            Email = "ana@example.com",
            PhoneNumber = "912345681",
            Address = "Rua da Ana",
            ZipCode = "1234-570"
        };

        // Act
        var result = await _controller.RegisterPatient(tutorId, newPatient);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var response = Assert.IsType<ApiResponse<PatientResponseDto>>(createdAtActionResult.Value);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(newPatient.Name, response.Data.Name);
        Assert.Equal(newPatient.Email, response.Data.Email);
    }

    /// <summary>
    /// Test that verifies if the `RegisterPatient` method returns a NotFound when the tutor does not exist
    /// </summary>
    [Fact]
    public async Task RegisterPatient_ReturnsNotFound_WhenTutorNotExists()
    {
        var tutorId = 999;
        var newPatient = new PatientRequestDto
        {
            Name = "Ana Pereira",
            Email = "ana@example.com",
            PhoneNumber = "912345681",
            Address = "Rua da Ana",
            ZipCode = "1234-570"
        };

        // Act
        var result = await _controller.RegisterPatient(tutorId, newPatient);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<PatientResponseDto>>(notFoundResult.Value);
        Assert.False(response.Success);
        Assert.Equal("Não existe nenhum tutor com esse id", response.Message);
    }

    /// <summary>
    /// Test that verifies if the `RegisterPatient` method returns a BadRequest when model state is invalid
    /// </summary>
    [Fact]
    public async Task RegisterPatient_ReturnsBadRequest_WhenModelStateInvalid()
    {
        var tutorId = 1;
        var newPatient = new PatientRequestDto
        {
            Name = "",
            Email = "ana@example.com",
            PhoneNumber = "912345681",
            Address = "Rua da Ana",
            ZipCode = "1234-570"
        };

        _controller.ModelState.AddModelError("Name", "Name is required");

        // Act
        var result = await _controller.RegisterPatient(tutorId, newPatient);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<PatientResponseDto>>(badRequestResult.Value);
        Assert.False(response.Success);
        Assert.Equal("Erros de validação", response.Message);
    }
}
