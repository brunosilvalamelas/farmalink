using Backend.Controllers;
using Backend.Data;
using Backend.DTOs.request;
using Backend.DTOs.response;
using Backend.Entities;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendTests.Integration.Controllers;

public class PatientsControllerTests : IDisposable
{
    private readonly DataContext _dataContext;
    private readonly PatientService _patientService;
    private readonly PatientsController _controller;

    /// <summary>
    /// Assigns the context to the PatientsControllerTests class
    /// </summary>
    public PatientsControllerTests()
    {
        // Initialize in-memory database
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "TestPatientDb")
            .Options;

        _dataContext = new DataContext(options);

        // Seed data
        var tutor = new Tutor
        {
            Id = 1,
            Name = "João Silva",
            Email = "joao@example.com",
            PhoneNumber = "912345678",
            Address = "Rua do João",
            ZipCode = "1234-567"
        };

        var patient1 = new Patient
        {
            Id = 1,
            Name = "Maria Santos",
            Email = "maria@example.com",
            PhoneNumber = "912345679",
            Address = "Rua da Maria",
            ZipCode = "1234-568",
            TutorId = 1,
            Tutor = tutor
        };

        var patient2 = new Patient
        {
            Id = 2,
            Name = "Pedro Costa",
            Email = "pedro@example.com",
            PhoneNumber = "912345680",
            Address = "Rua do Pedro",
            ZipCode = "1234-569",
            TutorId = 1,
            Tutor = tutor
        };

        _dataContext.Tutors.Add(tutor);
        _dataContext.Patients.AddRange(patient1, patient2);
        _dataContext.SaveChanges();

        _patientService = new PatientService(_dataContext);
        _controller = new PatientsController(_patientService);
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
    /// Test that verifies if the `GetAllPatients` method returns an OK result with a list of patients
    /// </summary>
    [Fact]
    public async Task GetAllPatients_ReturnsOkResult_WithListOfPatients()
    {
        // Act
        var result = await _controller.GetAllPatients();

        // Assert
        Assert.IsType<ActionResult<ApiResponse<List<PatientResponseDto>>>>(result);
    }

    /// <summary>
    /// Test that verifies if the `GetPatientsByTutorId` method returns an OK result with a list of patients
    /// </summary>
    [Fact]
    public async Task GetPatientsByTutorId_ReturnsOkResult_WithListOfPatients()
    {
        var tutorId = 1;

        // Act
        var result = await _controller.GetPatientsByTutorId(tutorId);

        // Assert
        Assert.IsType<ActionResult<ApiResponse<List<PatientOfTutorResponseDto>>>>(result);
    }

    /// <summary>
    /// Test that verifies if the `GetPatientsByTutorId` method returns a NotFound result when the tutor is not found
    /// </summary>
    [Fact]
    public async Task GetPatientsByTutorId_ReturnsNotFound_WhenTutorNotFound()
    {
        var tutorId = 999;

        // Act
        var result = await _controller.GetPatientsByTutorId(tutorId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<PatientOfTutorResponseDto>>(notFoundResult.Value);
        Assert.False(response.Success);
        Assert.Equal("Não existe nenhum tutor com esse id", response.Message);
    }

    /// <summary>
    /// Test that verifies if the `GetPatientById` method returns an OK result with a patient
    /// </summary>
    [Fact]
    public async Task GetPatientById_ReturnsOkResult_WithPatient()
    {
        var patientId = 1;

        // Act
        var result = await _controller.GetPatientById(patientId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<PatientResponseDto>>(okResult.Value);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(1, response.Data.Id);
    }

    /// <summary>
    /// Test that verifies if the `GetPatientById` method returns a NotFound result when the patient is not found
    /// </summary>
    [Fact]
    public async Task GetPatientById_ReturnsNotFound_WhenPatientNotFound()
    {
        var patientId = 999;

        // Act
        var result = await _controller.GetPatientById(patientId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<PatientResponseDto>>(notFoundResult.Value);
        Assert.False(response.Success);
        Assert.Equal("Não existe nenhum utente com esse id", response.Message);
    }

    /// <summary>
    /// Test that verifies if the `CreatePatient` method returns a CreatedAtActionResult when the patient is added
    /// </summary>
    [Fact]
    public async Task CreatePatient_ReturnsCreatedAtActionResult_WhenPatientAdded()
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
        var result = await _controller.CreatePatient(tutorId, newPatient);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var response = Assert.IsType<ApiResponse<PatientResponseDto>>(createdAtActionResult.Value);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(newPatient.Name, response.Data.Name);
        Assert.Equal(newPatient.Email, response.Data.Email);
    }

    /// <summary>
    /// Test that verifies if the `CreatePatient` method returns a NotFound when the tutor does not exist
    /// </summary>
    [Fact]
    public async Task CreatePatient_ReturnsNotFound_WhenTutorNotExists()
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
        var result = await _controller.CreatePatient(tutorId, newPatient);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<PatientResponseDto>>(notFoundResult.Value);
        Assert.False(response.Success);
        Assert.Equal("Não existe nenhum tutor com esse id", response.Message);
    }

    /// <summary>
    /// Test that verifies if the `CreatePatient` method returns a BadRequest when model state is invalid
    /// </summary>
    [Fact]
    public async Task CreatePatient_ReturnsBadRequest_WhenModelStateInvalid()
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
        var result = await _controller.CreatePatient(tutorId, newPatient);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<PatientResponseDto>>(badRequestResult.Value);
        Assert.False(response.Success);
        Assert.Equal("Erros de validação", response.Message);
    }

    /// <summary>
    /// Test that verifies if the `UpdatePatientById` method returns an OkObjectResult when the update is successful
    /// </summary>
    [Fact]
    public async Task UpdatePatientById_ReturnsOkResult_WhenUpdateSucceeds()
    {
        var patientId = 1;
        var updatedPatient = new PatientRequestDto
        {
            Name = "Updated Maria",
            Email = "updated@example.com",
            PhoneNumber = "965432109",
            Address = "Updated Address",
            ZipCode = "9876-543"
        };

        // Act
        var result = await _controller.UpdatePatientById(patientId, updatedPatient);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal("Os dados do utente foram atualizados", response.Message);
    }

    /// <summary>
    /// Test that verifies if the `UpdatePatientById` method returns a BadRequest when model state is invalid
    /// </summary>
    [Fact]
    public async Task UpdatePatientById_ReturnsBadRequest_WhenModelStateInvalid()
    {
        var patientId = 1;
        var updatedPatient = new PatientRequestDto
        {
            Name = "",
            Email = "updated@example.com",
            PhoneNumber = "965432109",
            Address = "Updated Address",
            ZipCode = "9876-543"
        };

        _controller.ModelState.AddModelError("Name", "Name is required");

        // Act
        var result = await _controller.UpdatePatientById(patientId, updatedPatient);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(badRequestResult.Value);
        Assert.False(response.Success);
        Assert.Equal("Erros de validação", response.Message);
    }

    /// <summary>
    /// Test that verifies if the `UpdatePatientById` method returns a NotFound when the patient does not exist
    /// </summary>
    [Fact]
    public async Task UpdatePatientById_ReturnsNotFound_WhenPatientNotFound()
    {
        var patientId = 999;
        var updatedPatient = new PatientRequestDto
        {
            Name = "Updated Maria",
            Email = "updated@example.com",
            PhoneNumber = "965432109",
            Address = "Updated Address",
            ZipCode = "9876-543"
        };

        // Act
        var result = await _controller.UpdatePatientById(patientId, updatedPatient);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(notFoundResult.Value);
        Assert.False(response.Success);
        Assert.Equal("Não existe nenhum utente com esse id", response.Message);
    }

    /// <summary>
    /// Test that verifies if the `DeletePatientById` method returns an OkObjectResult when the deletion is successful
    /// </summary>
    [Fact]
    public async Task DeletePatientById_ReturnsOkResult_WhenDeletionSucceeds()
    {
        var patientId = 1;

        // Act
        var result = await _controller.DeletePatientById(patientId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal("Utente removido", response.Message);
    }

    /// <summary>
    /// Test that verifies if the `DeletePatientById` method returns a NotFound when the patient does not exist
    /// </summary>
    [Fact]
    public async Task DeletePatientById_ReturnsNotFound_WhenPatientNotFound()
    {
        var patientId = 999;

        // Act
        var result = await _controller.DeletePatientById(patientId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(notFoundResult.Value);
        Assert.False(response.Success);
        Assert.Equal("Não existe nenhum utente com esse id", response.Message);
    }
}