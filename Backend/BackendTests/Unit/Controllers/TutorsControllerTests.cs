using Backend.Controllers;
using Backend.DTOs.request;
using Backend.DTOs.response;
using Backend.Entities;
using Backend.Exceptions;
using Backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BackendTests.Unit.Controllers;

public class TutorsControllerTests
{
    private readonly Mock<ITutorService> _mockTutorService;
    private readonly TutorsController _controller;

    public TutorsControllerTests()
    {
        _mockTutorService = new Mock<ITutorService>();
        _controller = new TutorsController(_mockTutorService.Object);
    }

    [Fact]
    public async Task CreateTutor_ReturnsCreatedAtAction_WhenValid()
    {
        // Arrange
        var tutorDto = new CreateTutorRequestDto
        {
            Name = "João Silva",
            Email = "joao@example.com",
            PhoneNumber = "912345678",
            Password = "password123",
            Address = "Rua do João",
            ZipCode = "1234-567"
        };
        var (createdTutor, token) = new Tuple<Tutor, string>(
            new Tutor
            {
                Id = 1,
                Name = tutorDto.Name,
                Email = tutorDto.Email,
                PhoneNumber = tutorDto.PhoneNumber,
                Address = tutorDto.Address,
                ZipCode = tutorDto.ZipCode
            },
            "token"
        );

        _mockTutorService
            .Setup(s => s.CreateTutorAsync(tutorDto))
            .ReturnsAsync((createdTutor, token));

        var httpContext = new DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

        // Act
        var result = await _controller.CreateTutor(tutorDto);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ApiResponse<CreateTutorResponseDto>>>(result);
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<CreateTutorResponseDto>>(createdAtActionResult.Value);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(createdTutor.Name, response.Data.Name);
        Assert.Equal("Tutor", response.Data.Role.ToString());
    }

    [Fact]
    public async Task CreateTutor_ReturnsBadRequest_WhenModelStateInvalid()
    {
        // Arrange
        var tutorDto = new CreateTutorRequestDto
        {
            Name = "",
            Email = "joao@example.com",
            PhoneNumber = "912345678",
            Password = "password123",
            Address = "Rua do João",
            ZipCode = "1234-567"
        };

        _controller.ModelState.AddModelError("Name", "Name is required");

        // Act
        var result = await _controller.CreateTutor(tutorDto);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ApiResponse<CreateTutorResponseDto>>>(result);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<CreateTutorResponseDto>>(badRequestResult.Value);
        Assert.False(response.Success);
        Assert.Equal("Erros de validação", response.Message);
    }

    [Fact]
    public async Task CreateTutor_ReturnsConflict_WhenValidationExceptionThrown()
    {
        // Arrange
        var tutorDto = new CreateTutorRequestDto
        {
            Name = "João Silva",
            Email = "duplicate@example.com",
            PhoneNumber = "912345678",
            Password = "password123",
            Address = "Rua do João",
            ZipCode = "1234-567"
        };

        _mockTutorService
            .Setup(s => s.CreateTutorAsync(tutorDto))
            .ThrowsAsync(new ValidationException(new List<ValidationError>()));

        var httpContext = new DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

        // Act
        var result = await _controller.CreateTutor(tutorDto);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ApiResponse<CreateTutorResponseDto>>>(result);
        var conflictResult = Assert.IsType<ConflictObjectResult>(actionResult.Result);
        Assert.Equal(409, conflictResult.StatusCode);
        var response = Assert.IsType<ApiResponse<CreateTutorResponseDto>>(conflictResult.Value);
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
        var tutorDto = new UpdateTutorRequestDto
        {
            Name = "Updated João",
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
        var tutorDto = new UpdateTutorRequestDto
        {
            Name = "",
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
        var tutorDto = new UpdateTutorRequestDto
        {
            Name = "Updated João",
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
}
