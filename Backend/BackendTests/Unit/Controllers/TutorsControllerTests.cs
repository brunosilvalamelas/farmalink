using Backend.Controllers;
using Backend.DTOs.request;
using Backend.DTOs.response;
using Backend.Entities;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BackendTests.Unit.Controllers;

public class TutorsControllerTests
{
    private readonly Mock<ITutorService> _mockTutorService;
    private readonly Mock<IPatientService> _mockPatientService;
    private readonly TutorsController _controller;

    public TutorsControllerTests()
    {
        _mockTutorService = new Mock<ITutorService>();
        _mockPatientService = new Mock<IPatientService>();
        _controller = new TutorsController(_mockTutorService.Object, _mockPatientService.Object);
    }

    [Fact]
    public async Task CreateTutor_ReturnsCreatedAtAction_WhenValid()
    {
        // Arrange
        var tutorDto = new TutorRequestDto
        {
            Name = "João Silva",
            Email = "joao@example.com",
            PhoneNumber = "912345678",
            Address = "Rua do João",
            ZipCode = "1234-567"
        };
        var createdTutor = new Tutor
        {
            Id = 1,
            Name = tutorDto.Name,
            Email = tutorDto.Email,
            PhoneNumber = tutorDto.PhoneNumber,
            Address = tutorDto.Address,
            ZipCode = tutorDto.ZipCode
        };

        _mockTutorService
            .Setup(s => s.CreateTutorAsync(tutorDto))
            .ReturnsAsync(createdTutor);

        // Act
        var result = await _controller.CreateTutor(tutorDto);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ApiResponse<TutorResponseDto>>>(result);
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<TutorResponseDto>>(createdAtActionResult.Value);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(createdTutor.Id, response.Data.Id);
    }

    [Fact]
    public async Task CreateTutor_ReturnsBadRequest_WhenModelStateInvalid()
    {
        // Arrange
        var tutorDto = new TutorRequestDto
        {
            Name = "",
            Email = "joao@example.com",
            PhoneNumber = "912345678",
            Address = "Rua do João",
            ZipCode = "1234-567"
        };

        _controller.ModelState.AddModelError("Name", "Name is required");

        // Act
        var result = await _controller.CreateTutor(tutorDto);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ApiResponse<TutorResponseDto>>>(result);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<TutorResponseDto>>(badRequestResult.Value);
        Assert.False(response.Success);
        Assert.Equal("Erros de validação", response.Message);
    }

    [Fact]
    public async Task GetAllTutors_ReturnsOkWithList()
    {
        // Arrange
        var tutors = new List<Tutor>
        {
            new Tutor { Id = 1, Name = "João Silva" }
        };

        _mockTutorService
            .Setup(s => s.GetAllTutorsAsync())
            .ReturnsAsync(tutors);

        // Act
        var result = await _controller.GetAllTutors();

        // Assert
        var actionResult = Assert.IsType<ActionResult<ApiResponse<List<TutorResponseDto>>>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<List<TutorResponseDto>>>(okResult.Value);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Single(response.Data);
    }

    [Fact]
    public async Task GetTutorById_ReturnsOkWithTutor_WhenFound()
    {
        // Arrange
        var tutorId = 1;
        var tutor = new Tutor
        {
            Id = tutorId,
            Name = "João Silva"
        };

        _mockTutorService
            .Setup(s => s.GetTutorByIdAsync(tutorId))
            .ReturnsAsync(tutor);

        // Act
        var result = await _controller.GetTutorById(tutorId);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ApiResponse<TutorResponseDto>>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<TutorResponseDto>>(okResult.Value);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(tutor.Id, response.Data.Id);
    }

    [Fact]
    public async Task GetTutorById_ReturnsNotFound_WhenNotFound()
    {
        // Arrange
        var tutorId = 999;

        _mockTutorService
            .Setup(s => s.GetTutorByIdAsync(tutorId))
            .ReturnsAsync((Tutor?)null);

        // Act
        var result = await _controller.GetTutorById(tutorId);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ApiResponse<TutorResponseDto>>>(result);
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<TutorResponseDto>>(notFoundResult.Value);
        Assert.False(response.Success);
        Assert.Equal("Não existe nenhum tutor com esse id", response.Message);
    }

    [Fact]
    public async Task UpdateTutorById_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var tutorId = 1;
        var tutorDto = new TutorRequestDto
        {
            Name = "Updated João",
            Email = "updated@example.com",
            PhoneNumber = "912345678",
            Address = "Updated Address",
            ZipCode = "1234-567"
        };

        _mockTutorService
            .Setup(s => s.UpdateTutorAsync(tutorId, tutorDto))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.UpdateTutorById(tutorId, tutorDto);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ApiResponse<bool>>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<bool>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal("Os dados do tutor foram atualizados", response.Message);
    }

    [Fact]
    public async Task UpdateTutorById_ReturnsBadRequest_WhenModelStateInvalid()
    {
        // Arrange
        var tutorId = 1;
        var tutorDto = new TutorRequestDto
        {
            Name = "",
            Email = "updated@example.com",
            PhoneNumber = "912345678",
            Address = "Updated Address",
            ZipCode = "1234-567"
        };

        _controller.ModelState.AddModelError("Name", "Name is required");

        // Act
        var result = await _controller.UpdateTutorById(tutorId, tutorDto);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ApiResponse<bool>>>(result);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<bool>>(badRequestResult.Value);
        Assert.False(response.Success);
        Assert.Equal("Erros de validação", response.Message);
    }

    [Fact]
    public async Task UpdateTutorById_ReturnsNotFound_WhenTutorDoesNotExist()
    {
        // Arrange
        var tutorId = 999;
        var tutorDto = new TutorRequestDto
        {
            Name = "Updated João",
            Email = "updated@example.com",
            PhoneNumber = "912345678",
            Address = "Updated Address",
            ZipCode = "1234-567"
        };

        _mockTutorService
            .Setup(s => s.UpdateTutorAsync(tutorId, tutorDto))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.UpdateTutorById(tutorId, tutorDto);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ApiResponse<bool>>>(result);
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<bool>>(notFoundResult.Value);
        Assert.False(response.Success);
        Assert.Equal("Não existe nenhum tutor com esse id", response.Message);
    }

    [Fact]
    public async Task DeleteTutorById_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var tutorId = 1;

        _mockTutorService
            .Setup(s => s.DeleteTutorAsync(tutorId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteTutorById(tutorId);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ApiResponse<bool>>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<bool>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal("Tutor removido", response.Message);
    }

    [Fact]
    public async Task DeleteTutorById_ReturnsNotFound_WhenTutorDoesNotExist()
    {
        // Arrange
        var tutorId = 999;

        _mockTutorService
            .Setup(s => s.DeleteTutorAsync(tutorId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteTutorById(tutorId);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ApiResponse<bool>>>(result);
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<bool>>(notFoundResult.Value);
        Assert.False(response.Success);
        Assert.Equal("Não existe nenhum tutor com esse id", response.Message);
    }

    [Fact]
    public async Task RegisterPatient_ReturnsCreatedAtAction_WhenValid()
    {
        // Arrange
        var tutorId = 1;
        var patientDto = new PatientRequestDto
        {
            Name = "Maria Santos",
            Email = "maria@example.com",
            PhoneNumber = "912345679",
            Address = "Rua da Maria",
            ZipCode = "1234-568"
        };
        var createdPatient = new Patient
        {
            Id = 2,
            Name = patientDto.Name,
            Email = patientDto.Email,
            PhoneNumber = patientDto.PhoneNumber,
            Address = patientDto.Address,
            ZipCode = patientDto.ZipCode,
            TutorId = tutorId
        };

        _mockPatientService
            .Setup(s => s.CreatePatientAsync(tutorId, patientDto))
            .ReturnsAsync(createdPatient);

        // Act
        var result = await _controller.RegisterPatient(tutorId, patientDto);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ApiResponse<PatientResponseDto>>>(result);
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<PatientResponseDto>>(createdAtActionResult.Value);
        Assert.True(response.Success);

        Assert.NotNull(response.Data);
        Assert.Equal(createdPatient.Id, response.Data.Id);
    }

    [Fact]
    public async Task RegisterPatient_ReturnsNotFound_WhenTutorDoesNotExist()
    {
        // Arrange
        var tutorId = 999;
        var patientDto = new PatientRequestDto
        {
            Name = "Maria Santos",
            Email = "maria@example.com",
            PhoneNumber = "912345679",
            Address = "Rua da Maria",
            ZipCode = "1234-568"
        };

        _mockPatientService
            .Setup(s => s.CreatePatientAsync(tutorId, patientDto))
            .ReturnsAsync((Patient?)null);

        // Act
        var result = await _controller.RegisterPatient(tutorId, patientDto);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ApiResponse<PatientResponseDto>>>(result);
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<PatientResponseDto>>(notFoundResult.Value);
        Assert.False(response.Success);
        Assert.Equal("Não existe nenhum tutor com esse id", response.Message);
    }

    [Fact]
    public async Task RegisterPatient_ReturnsBadRequest_WhenModelStateInvalid()
    {
        // Arrange
        var tutorId = 1;
        var patientDto = new PatientRequestDto
        {
            Name = "",
            Email = "maria@example.com",
            PhoneNumber = "912345679",
            Address = "Rua da Maria",
            ZipCode = "1234-568"
        };

        _controller.ModelState.AddModelError("Name", "Name is required");

        // Act
        var result = await _controller.RegisterPatient(tutorId, patientDto);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ApiResponse<PatientResponseDto>>>(result);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<PatientResponseDto>>(badRequestResult.Value);
        Assert.False(response.Success);
        Assert.Equal("Erros de validação", response.Message);
    }
}