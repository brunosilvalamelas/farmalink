using Backend.Controllers;
using Backend.DTOs.request;
using Backend.DTOs.response;
using Backend.Entities;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Backend.Exceptions;
using Moq;

namespace BackendTests.Unit.Controllers;

public class PatientsControllerTests
{
    private readonly Mock<IPatientService> _mockPatientService;
    private readonly PatientsController _controller;

    public PatientsControllerTests()
    {
        _mockPatientService = new Mock<IPatientService>();
        _controller = new PatientsController(_mockPatientService.Object);
    }

    [Fact]
    public async Task CreatePatient_ReturnsCreatedAtAction_WhenValid()
    {
        // Arrange
        var patientDto = new CreatePatientRequestDto
        {
            Name = "Maria Santos",
            Email = "maria@example.com",
            PhoneNumber = "912345679",
            Address = "Rua da Maria",
            ZipCode = "1234-568"
        };
        var createdPatient = new Patient
        {
            Id = 1,
            Name = patientDto.Name,
            Email = patientDto.Email,
            PhoneNumber = patientDto.PhoneNumber,
            Address = patientDto.Address,
            ZipCode = patientDto.ZipCode,
            TutorId = 1
        };

        _mockPatientService
            .Setup(s => s.CreatePatientAsync(1, patientDto))
            .ReturnsAsync(createdPatient);

        var claims = new List<Claim>() { new Claim(ClaimTypes.NameIdentifier, "1") };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = claimsPrincipal };
        _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

        // Act
        var result = await _controller.CreatePatient(patientDto);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ApiResponse<PatientResponseDto>>>(result);
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<PatientResponseDto>>(createdAtActionResult.Value!);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(createdPatient.Id, response.Data.Id);
    }
    [Fact]
       public async Task CreatePatient_ReturnsUnauthorized_WhenNoAuthenticatedTutor()
       {
           // Arrange
           var patientDto = new CreatePatientRequestDto
           {
               Name = "Maria Santos",
               Email = "maria@example.com",
               PhoneNumber = "912345679",
               Address = "Rua da Maria",
               ZipCode = "1234-568"
           };

           // No claims, or invalid
           var httpContext = new DefaultHttpContext { User = null };
           _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

           // Act
           var result = await _controller.CreatePatient(patientDto);

           // Assert
           var actionResult = Assert.IsType<ActionResult<ApiResponse<PatientResponseDto>>>(result);
           var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
           var response = Assert.IsType<ApiResponse<PatientResponseDto>>(unauthorizedResult.Value!);
           Assert.False(response.Success);
           Assert.Equal("Nenhum tutor autenticado", response.Message);
       }
    
       [Fact]
       public async Task CreatePatient_ReturnsBadRequest_WhenModelStateInvalid()
       {
           // Arrange
           var patientDto = new CreatePatientRequestDto
           {
               Name = "",
               Email = "maria@example.com",
               PhoneNumber = "912345679",
               Address = "Rua da Maria",
               ZipCode = "1234-568"
           };

           _controller.ModelState.AddModelError("Name", "Name is required");

           // Act
           var result = await _controller.CreatePatient(patientDto);

           // Assert
           var actionResult = Assert.IsType<ActionResult<ApiResponse<PatientResponseDto>>>(result);
           var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
           var response = Assert.IsType<ApiResponse<PatientResponseDto>>(badRequestResult.Value!);
           Assert.False(response.Success);
           Assert.Equal("Erros de validação", response.Message);
       }

       [Fact]
       public async Task CreatePatient_ReturnsConflict_WhenValidationExceptionThrown()
       {
           // Arrange
           var patientDto = new CreatePatientRequestDto
           {
               Name = "Maria Santos",
               Email = "maria@example.com",
               PhoneNumber = "912345679",
               Address = "Rua da Maria",
               ZipCode = "1234-568"
           };

           var claims = new List<Claim>() { new Claim(ClaimTypes.NameIdentifier, "1") };
           var identity = new ClaimsIdentity(claims, "TestAuthType");
           var claimsPrincipal = new ClaimsPrincipal(identity);
           var httpContext = new DefaultHttpContext { User = claimsPrincipal };
           _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

           _mockPatientService
               .Setup(s => s.CreatePatientAsync(1, patientDto))
               .ThrowsAsync(new ValidationException(new List<ValidationError>()));

           // Act
           var result = await _controller.CreatePatient(patientDto);

           // Assert
           var actionResult = Assert.IsType<ActionResult<ApiResponse<PatientResponseDto>>>(result);
           var conflictResult = Assert.IsType<ConflictObjectResult>(actionResult.Result);
           Assert.Equal(409, conflictResult.StatusCode);
           var response = Assert.IsType<ApiResponse<PatientResponseDto>>(conflictResult.Value!);
           Assert.False(response.Success);
           Assert.Equal("Erros de validação", response.Message);
       }

       [Fact]
       public async Task CreatePatient_ReturnsNotFound_WhenTutorDoesNotExist()
       {
           // Arrange
           var patientDto = new CreatePatientRequestDto
           {
               Name = "Maria Santos",
               Email = "maria@example.com",
               PhoneNumber = "912345679",
               Address = "Rua da Maria",
               ZipCode = "1234-568"
           };

           var claims = new List<Claim>() { new Claim(ClaimTypes.NameIdentifier, "1") };
           var identity = new ClaimsIdentity(claims, "TestAuthType");
           var claimsPrincipal = new ClaimsPrincipal(identity);
           var httpContext = new DefaultHttpContext { User = claimsPrincipal };
           _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

           _mockPatientService
               .Setup(s => s.CreatePatientAsync(1, patientDto))
               .ReturnsAsync((Patient?)null);

           // Act
           var result = await _controller.CreatePatient(patientDto);

           // Assert
           var actionResult = Assert.IsType<ActionResult<ApiResponse<PatientResponseDto>>>(result);
           var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
           var response = Assert.IsType<ApiResponse<PatientResponseDto>>(notFoundResult.Value!);
           Assert.False(response.Success);
           Assert.Equal("Tutor não encontrado", response.Message);
       }

    [Fact]
    public async Task GetAllPatients_ReturnsOkWithList()
    {
        // Arrange
        var patients = new List<Patient>
        {
            new Patient { Id = 1, Name = "Maria Santos", Tutor = new Tutor { Name = "João" } }
        };

        _mockPatientService
            .Setup(s => s.GetAllPatientsAsync())
            .ReturnsAsync(patients);

        var claims = new List<Claim>() { new Claim(ClaimTypes.Role, "Tutor") };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = claimsPrincipal };
        _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

        // Act
        var result = await _controller.GetAllPatients();

        // Assert
        var actionResult = Assert.IsType<ActionResult<ApiResponse<List<PatientResponseDto>>>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<List<PatientResponseDto>>>(okResult.Value!);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Single(response.Data);
    }

    [Fact]
    public async Task GetPatientsByTutorId_ReturnsOkWithList_WhenTutorExists()
    {
        // Arrange
        var tutorId = 1;
        var patients = new List<Patient>
        {
            new Patient { Id = 1, Name = "Maria Santos" }
        };

        _mockPatientService
            .Setup(s => s.GetPatientsByTutorIdAsync(tutorId))
            .ReturnsAsync(patients);

        // Act
        var result = await _controller.GetPatientsByTutorId(tutorId);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ApiResponse<List<PatientOfTutorResponseDto>>>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<List<PatientOfTutorResponseDto>>>(okResult.Value!);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Single(response.Data);
    }

    [Fact]
    public async Task GetPatientsByTutorId_ReturnsNotFound_WhenTutorDoesNotExist()
    {
        // Arrange
        var tutorId = 999;

        _mockPatientService
            .Setup(s => s.GetPatientsByTutorIdAsync(tutorId))
            .ReturnsAsync((List<Patient>?)null);

        // Act
        var result = await _controller.GetPatientsByTutorId(tutorId);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ApiResponse<List<PatientOfTutorResponseDto>>>>(result);
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<PatientOfTutorResponseDto>>(notFoundResult.Value!);
        Assert.False(response.Success);
        Assert.Equal("Não existe nenhum tutor com esse id", response.Message);
    }

    [Fact]
    public async Task GetPatientById_ReturnsOkWithPatient_WhenFound()
    {
        // Arrange
        var patientId = 1;
        var patient = new Patient
        {
            Id = patientId,
            Name = "Maria Santos",
            Tutor = new Tutor { Name = "João" }
        };

        _mockPatientService
            .Setup(s => s.GetPatientByIdAsync(patientId))
            .ReturnsAsync(patient);

        // Act
        var result = await _controller.GetPatientById(patientId);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ApiResponse<PatientResponseDto>>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<PatientResponseDto>>(okResult.Value!);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(patient.Id, response.Data.Id);
    }

    [Fact]
    public async Task GetPatientById_ReturnsNotFound_WhenNotFound()
    {
        // Arrange
        var patientId = 999;

        _mockPatientService
            .Setup(s => s.GetPatientByIdAsync(patientId))
            .ReturnsAsync((Patient?)null);

        // Act
        var result = await _controller.GetPatientById(patientId);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ApiResponse<PatientResponseDto>>>(result);
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<PatientResponseDto>>(notFoundResult.Value!);
        Assert.False(response.Success);
        Assert.Equal("Não existe nenhum utente com esse id", response.Message);
    }

    [Fact]
    public async Task UpdatePatientById_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var patientId = 1;
        var patientDto = new UpdatePatientRequestDto
        {
            Name = "Updated Maria",
            Email = "updated@example.com",
            PhoneNumber = "965432109",
            Address = "Updated Address",
            ZipCode = "9876-543"
        };

        _mockPatientService
            .Setup(s => s.UpdatePatientAsync(patientId, patientDto))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.UpdatePatientById(patientId, patientDto);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ApiResponse<bool>>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<bool>>(okResult.Value!);
        Assert.True(response.Success);
        Assert.Equal("Os dados do utente foram atualizados", response.Message);
    }

    [Fact]
    public async Task UpdatePatientById_ReturnsBadRequest_WhenModelStateInvalid()
    {
        // Arrange
        var patientId = 1;
        var patientDto = new UpdatePatientRequestDto
        {
            Name = "",
            Email = "updated@example.com",
            PhoneNumber = "965432109",
            Address = "Updated Address",
            ZipCode = "9876-543"
        };

        _controller.ModelState.AddModelError("Name", "Name is required");

        // Act
        var result = await _controller.UpdatePatientById(patientId, patientDto);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ApiResponse<bool>>>(result);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<bool>>(badRequestResult.Value!);
        Assert.False(response.Success);
        Assert.Equal("Erros de validação", response.Message);
    }

    [Fact]
    public async Task UpdatePatientById_ReturnsNotFound_WhenPatientDoesNotExist()
    {
        // Arrange
        var patientId = 999;
        var patientDto = new UpdatePatientRequestDto
        {
            Name = "Updated Maria",
            Email = "updated@example.com",
            PhoneNumber = "965432109",
            Address = "Updated Address",
            ZipCode = "9876-543"
        };

        _mockPatientService
            .Setup(s => s.UpdatePatientAsync(patientId, patientDto))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.UpdatePatientById(patientId, patientDto);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ApiResponse<bool>>>(result);
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<bool>>(notFoundResult.Value!);
        Assert.False(response.Success);
        Assert.Equal("Não existe nenhum utente com esse id", response.Message);
    }

    [Fact]
    public async Task DeletePatientById_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var patientId = 1;

        _mockPatientService
            .Setup(s => s.DeletePatientAsync(patientId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeletePatientById(patientId);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ApiResponse<bool>>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<bool>>(okResult.Value!);
        Assert.True(response.Success);
        Assert.Equal("Utente removido", response.Message);
    }

    [Fact]
    public async Task DeletePatientById_ReturnsNotFound_WhenPatientDoesNotExist()
    {
        // Arrange
        var patientId = 999;

        _mockPatientService
            .Setup(s => s.DeletePatientAsync(patientId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.DeletePatientById(patientId);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ApiResponse<bool>>>(result);
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<bool>>(notFoundResult.Value!);
        Assert.False(response.Success);
        Assert.Equal("Não existe nenhum utente com esse id", response.Message);
    }
}
