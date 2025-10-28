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
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dataContext = new DataContext(options);

        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(c => c["Jwt:Key"]).Returns("SuperSecretKeyForTesting");
        var userService = new UserService(mockConfig.Object, _dataContext);

        _patientService = new PatientService(_dataContext, userService);
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

    private void SeedData()
    {
        var tutor = new Tutor
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

        var patient1 = new Patient
        {
            Id = 2,
            Name = "Maria Santos",
            Email = "maria@example.com",
            PhoneNumber = "912345679",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
            Address = "Rua da Maria",
            ZipCode = "1234-568",
            TutorId = 1,
            Role = UserRole.Patient
        };

        var patient2 = new Patient
        {
            Id = 3,
            Name = "Pedro Costa",
            Email = "pedro@example.com",
            PhoneNumber = "912345680",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
            Address = "Rua do Pedro",
            ZipCode = "1234-569",
            TutorId = 1,
            Role = UserRole.Patient
        };

        _dataContext.Tutors.Add(tutor);
        _dataContext.Patients.AddRange(patient1, patient2);
        _dataContext.SaveChanges();
        _dataContext.ChangeTracker.Clear();
    }

    /// <summary>
    /// Test that verifies if the `GetAllPatients` method returns an OK result with a list of patients
    /// </summary>
    [Fact]
    public async Task GetAllPatients_ReturnsOkResult_WithListOfPatients()
    {
        SeedData();

        var claims = new List<Claim>()
            { new Claim(ClaimTypes.Role, "Tutor") };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = claimsPrincipal };
        _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

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
        SeedData();
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
        SeedData();
        var patientId = 2;

        // Act
        var result = await _controller.GetPatientById(patientId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<PatientResponseDto>>(okResult.Value);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(2, response.Data.Id);
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

    // /// <summary>
    // /// Test that verifies if the `CreatePatient` method returns a CreatedAtActionResult when the patient is added
    // /// </summary>
    [Fact]
    public async Task CreatePatient_ReturnsCreatedAtActionResult_WhenPatientAdded()
    {
        var newPatient = new CreatePatientRequestDto
        {
            Name = "Ana Pereira",
            Email = "ana@example.com",
            PhoneNumber = "912345681",
            Address = "Rua da Ana",
            ZipCode = "1234-570"
        };

        var claims = new List<Claim>()
            { new Claim(ClaimTypes.NameIdentifier, "1") };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = claimsPrincipal };
        _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

        // Seed tutor
        var tutor = new Tutor
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
        _dataContext.Tutors.Add(tutor);
        _dataContext.SaveChanges();
        _dataContext.ChangeTracker.Clear();

        // Act
        var result = await _controller.CreatePatient(newPatient);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var response = Assert.IsType<ApiResponse<PatientResponseDto>>(createdAtActionResult.Value);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(newPatient.Name, response.Data.Name);
        Assert.Equal(newPatient.Email, response.Data.Email);
    }

    /// <summary>
    /// Test that verifies if the `CreatePatient` method returns a BadRequest when the tutor does not exist
    /// </summary>
    // [Fact]
    // public async Task CreatePatient_ReturnsBadRequest_WhenTutorDoesNotExist()
    // {
    //     var newPatient = new CreatePatientRequestDto
    //     {
    //         Name = "Ana Pereira",
    //         Email = "ana@example.com",
    //         PhoneNumber = "912345681",
    //         Address = "Rua da Ana",
    //         ZipCode = "1234-570"
    //     };
    //
    //     var claims = new List<System.Security.Claims.Claim>() { new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, "abc") };
    //     var identity = new System.Security.Claims.ClaimsIdentity(claims, "TestAuthType");
    //     var claimsPrincipal = new System.Security.Claims.ClaimsPrincipal(identity);
    //     var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext { User = claimsPrincipal };
    //     _controller.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext { HttpContext = httpContext };
    //
    //     // Act
    //     var result = await _controller.CreatePatient(newPatient);
    //
    //     // Assert
    //     var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
    //     var response = Assert.IsType<ApiResponse<PatientResponseDto>>(unauthorizedResult.Value);
    //     Assert.False(response.Success);
    //     Assert.Equal("Nenhum tutor autenticado", response.Message);
    // }

    /// <summary>
    /// Test that verifies if the `CreatePatient` method returns a BadRequest when model state is invalid
    /// </summary>
    [Fact]
    public async Task CreatePatient_ReturnsBadRequest_WhenModelStateInvalid()
    {
        var newPatient = new CreatePatientRequestDto
        {
            Name = "",
            Email = "ana@example.com",
            PhoneNumber = "912345681",
            Address = "Rua da Ana",
            ZipCode = "1234-570"
        };

        _controller.ModelState.AddModelError("Name", "Name is required");

        // Act
        var result = await _controller.CreatePatient(newPatient);

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
        SeedData();
        var patientId = 2;
        var updatedPatient = new UpdatePatientRequestDto
        {
            Name = "Updated Maria",
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
        var updatedPatient = new UpdatePatientRequestDto
        {
            Name = "",
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
        var updatedPatient = new UpdatePatientRequestDto
        {
            Name = "Updated Maria",
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
        SeedData();
        var patientId = 2;

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