using Backend.DTOs.request;
using Backend.DTOs.response;
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
    private readonly TutorService _tutorService;
    private readonly PatientService _patientService;

    /// <summary>
    /// Initializes a new instance of the TutorsController.
    /// </summary>
    /// <param name="tutorService">The tutor service instance.</param>
    /// <param name="patientService">The patient service instance.</param>
    public TutorsController(TutorService tutorService, PatientService patientService)
    {
        _tutorService = tutorService;
        _patientService = patientService;
    }

    /// <summary>
    /// Creates a new tutor.
    /// </summary>
    /// <param name="tutorDto">The tutor data to create.</param>
    /// <returns>An IActionResult containing the created tutor or error information.</returns>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<TutorResponseDto>>> CreateTutor(
        [FromBody] TutorRequestDto tutorDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = GetModelStateErrors();
            return BadRequest(new ApiResponse<TutorResponseDto>
            {
                Success = false,
                Message = "Erros de validação",
                Errors = errors
            });
        }

        try
        {
            var createdTutor = await _tutorService.CreateTutorAsync(tutorDto);
            var tutorResponse = new TutorResponseDto
            {
                Id = createdTutor.Id,
                Name = createdTutor.Name,
                Email = createdTutor.Email,
                PhoneNumber = createdTutor.PhoneNumber,
                Address = createdTutor.Address,
                ZipCode = createdTutor.ZipCode
            };

            return CreatedAtAction(nameof(GetTutorById), new { id = tutorResponse.Id },
                new ApiResponse<TutorResponseDto>
                    { Success = true, Data = tutorResponse, Message = "Tutor criado" });
        }
        catch (ValidationException e)
        {
            return Conflict(new ApiResponse<TutorResponseDto>
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
    /// <param name="tutorDto">The updated tutor data.</param>
    /// <returns>An IActionResult containing the result of the update operation.</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateTutorById(int id,
        [FromBody] TutorRequestDto tutorDto)
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

        var updated = await _tutorService.UpdateTutorAsync(id, tutorDto);

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

    /// <summary>
    /// Registers a new patient for the specified tutor.
    /// </summary>
    /// <param name="tutorId">The ID of the tutor for whom to register the patient.</param>
    /// <param name="patientDto">The patient data to register.</param>
    /// <returns>An IActionResult containing the created patient or error information.</returns>
    [HttpPost("{tutorId}/patients")]
    public async Task<ActionResult<ApiResponse<PatientResponseDto>>> RegisterPatient(
        int tutorId,
        [FromBody] PatientRequestDto patientDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = GetModelStateErrors();
            return BadRequest(new ApiResponse<PatientResponseDto>
            {
                Success = false,
                Message = "Erros de validação",
                Errors = errors
            });
        }

        try
        {
            var patient = await _patientService.CreatePatientAsync(tutorId, patientDto);

            if (patient == null)
            {
                return NotFound(new ApiResponse<PatientResponseDto>
                {
                    Success = false,
                    Message = "Não existe nenhum tutor com esse id",
                });
            }

            var response = new PatientResponseDto
            {
                Id = patient.Id,
                Name = patient.Name,
                Email = patient.Email,
                PhoneNumber = patient.PhoneNumber,
                Address = patient.Address,
                ZipCode = patient.ZipCode
            };

            return CreatedAtAction(nameof(PatientsController.GetPatientById),
                "Patients",
                new { id = response.Id },
                new ApiResponse<PatientResponseDto> { Success = true, Data = response, Message = "Utente criado" });
        }
        catch (ValidationException e)
        {
            return Conflict(new ApiResponse<PatientResponseDto>
            {
                Success = false,
                Message = "Erros de validação",
                Errors = e.Errors
            });
        }
    }
}
