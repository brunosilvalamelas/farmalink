using Backend.DTOs.request;
using Backend.DTOs.response;
using Backend.Entities.Enums;
using Backend.Exceptions;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

/// <summary>
/// API controller for managing tutor-related operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TutorsController : BaseApiController
{
    private readonly ITutorService _tutorService;

    /// <summary>
    /// Initializes a new instance of the TutorsController.
    /// </summary>
    /// <param name="tutorService">The tutor service instance.</param>
    public TutorsController(ITutorService tutorService)
    {
        _tutorService = tutorService;
    }

    /// <summary>
    /// Creates a new tutor and generates an authentication token.
    /// </summary>
    /// <param name="createTutorDto">The tutor creation data.</param>
    /// <returns>An IActionResult containing the created tutor data and token or error information.</returns>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<CreateTutorResponseDto>>> CreateTutor(
        [FromBody] CreateTutorRequestDto createTutorDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = GetModelStateErrors();
            return BadRequest(new ApiResponse<CreateTutorResponseDto>
            {
                Success = false,
                Message = "Erros de validação",
                Errors = errors
            });
        }

        try
        {
            var (createdTutor, token) = await _tutorService.CreateTutorAsync(createTutorDto);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddMinutes(120)
            };

            Response.Cookies.Append("access_token", token, cookieOptions);

            var response = new CreateTutorResponseDto
            {
                Name = createdTutor.Name,
                Role = UserRole.Tutor
            };

            return CreatedAtAction(nameof(GetTutorById), new { id = createdTutor.Id },
                new ApiResponse<CreateTutorResponseDto>
                    { Success = true, Data = response, Message = "Tutor registado" });
        }
        catch (ValidationException e)
        {
            return Conflict(new ApiResponse<CreateTutorResponseDto>
                { Success = false, Message = "Erros de validação", Errors = e.Errors });
        }
    }


    /// <summary>
    /// Retrieves all tutors.
    /// </summary>
    /// <returns>An IActionResult containing the list of tutors or error information.</returns>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<TutorResponseDto>>>> GetAllTutors()
    {
        var tutors = await _tutorService.GetAllTutorsAsync();

        var tutorsResponse = tutors.Select(tutor => new TutorResponseDto
        {
            Id = tutor.Id,
            Name = tutor.Name,
            Email = tutor.Email,
            PhoneNumber = tutor.PhoneNumber,
            Address = tutor.Address,
            ZipCode = tutor.ZipCode,
        }).ToList();

        return Ok(new ApiResponse<List<TutorResponseDto>>
            { Message = "Tutores encontrados.", Data = tutorsResponse });
    }

    /// <summary>
    /// Retrieves a tutor by their ID.
    /// </summary>
    /// <param name="id">The ID of the tutor to retrieve.</param>
    /// <returns>An IActionResult containing the tutor data or error information.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<TutorResponseDto>>> GetTutorById(int id)
    {
        var tutor = await _tutorService.GetTutorByIdAsync(id);

        if (tutor == null)
        {
            return NotFound(new ApiResponse<TutorResponseDto>
                { Success = false, Message = "Não existe nenhum tutor com esse id" });
        }


        var tutorResponse =
            new TutorResponseDto
            {
                Id = tutor.Id,
                Name = tutor.Name,
                Email = tutor.Email,
                PhoneNumber = tutor.PhoneNumber,
                Address = tutor.Address,
                ZipCode = tutor.ZipCode,
            };
        return Ok(new ApiResponse<TutorResponseDto> { Message = "Tutor encontrado", Data = tutorResponse });
    }

    /// <summary>
    /// Updates an existing tutor by ID.
    /// </summary>
    /// <param name="id">The ID of the tutor to update.</param>
    /// <param name="updateTutorDto">The updated tutor data.</param>
    /// <returns>An IActionResult containing the result of the update operation.</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateTutorById(int id,
        [FromBody] UpdateTutorRequestDto updateTutorDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = GetModelStateErrors();
            return BadRequest(new ApiResponse<bool>
            {
                Success = false,
                Message = "Erros de validação",
                Errors = errors
            });
        }

        var updated = await _tutorService.UpdateTutorAsync(id, updateTutorDto);

        if (!updated)
        {
            return NotFound(
                new ApiResponse<bool> { Success = false, Message = "Não existe nenhum tutor com esse id" });
        }

        return Ok(new ApiResponse<bool> { Message = "Os dados do tutor foram atualizados" });
    }

    /// <summary>
    /// Deletes a tutor by their ID.
    /// </summary>
    /// <param name="id">The ID of the tutor to delete.</param>
    /// <returns>An IActionResult containing the result of the delete operation.</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteTutorById(int id)
    {
        var deleted = await _tutorService.DeleteTutorAsync(id);
        if (!deleted)
        {
            return NotFound(
                new ApiResponse<bool> { Success = false, Message = "Não existe nenhum tutor com esse id" });
        }

        return Ok(new ApiResponse<bool> { Message = "Tutor removido" });
    }
}