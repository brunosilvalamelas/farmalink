using Backend.DTOs.request;
using Backend.DTOs.response;
using Backend.Exceptions;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

/// <summary>
/// API controller for managing patient-related operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PatientsController : BaseApiController
{
    private readonly IPatientService _patientService;

    /// <summary>
    /// Initializes a new instance of the PatientsController.
    /// </summary>
    /// <param name="patientService">The patient service instance.</param>
    public PatientsController(IPatientService patientService)
    {
        _patientService = patientService;
    }

    /// <summary>
    /// Creates a new patient.
    /// </summary>
    /// <param name="tutorId">The ID of the tutor for the patient.</param>
    /// <param name="patientDto">The patient data to create.</param>
    /// <returns>An IActionResult containing the created patient or error information.</returns>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<PatientResponseDto>>> CreatePatient(int tutorId,
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
            var createdPatient = await _patientService.CreatePatientAsync(tutorId, patientDto);

            if (createdPatient == null)
            {
                return NotFound(new ApiResponse<PatientResponseDto>
                    { Success = false, Message = "Não existe nenhum tutor com esse id" });
            }

            var patientResponse = new PatientResponseDto
            {
                Id = createdPatient.Id,
                Name = createdPatient.Name,
                Email = createdPatient.Email,
                PhoneNumber = createdPatient.PhoneNumber,
                Address = createdPatient.Address,
                ZipCode = createdPatient.ZipCode
            };

            return CreatedAtAction(nameof(GetPatientById), new { id = patientResponse.Id },
                new ApiResponse<PatientResponseDto>
                    { Success = true, Data = patientResponse, Message = "Utente criado" });
        }
        catch (ValidationException e)
        {
            return Conflict(new ApiResponse<PatientResponseDto>
                { Success = false, Message = "Erros de validação", Errors = e.Errors });
        }
    }

    /// <summary>
    /// Retrieves all patients.
    /// </summary>
    /// <returns>An IActionResult containing the list of patients or error information.</returns>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<PatientResponseDto>>>> GetAllPatients()
    {
        var patients = await _patientService.GetAllPatientsAsync();

        var patientsResponse = patients.Select(patient => new PatientResponseDto
        {
            Id = patient.Id,
            Name = patient.Name,
            Email = patient.Email,
            PhoneNumber = patient.PhoneNumber,
            Address = patient.Address,
            ZipCode = patient.ZipCode,
            TutorId = patient.TutorId,
            TutorName = patient.Tutor.Name
        }).ToList();

        return Ok(new ApiResponse<List<PatientResponseDto>>
            { Message = "Utentes encontrados.", Data = patientsResponse });
    }

    /// <summary>
    /// Retrieves patients by tutor ID.
    /// </summary>
    /// <returns>An IActionResult containing the list of patients or error information.</returns>
    [HttpGet("by-tutor/{tutorId}")]
    public async Task<ActionResult<ApiResponse<List<PatientOfTutorResponseDto>>>> GetPatientsByTutorId(int tutorId)
    {
        var patients = await _patientService.GetPatientsByTutorIdAsync(tutorId);

        if (patients == null)
        {
            return NotFound(new ApiResponse<PatientOfTutorResponseDto>
                { Success = false, Message = "Não existe nenhum tutor com esse id" });
        }

        var patientsResponse = patients.Select(patient => new PatientOfTutorResponseDto
        {
            Id = patient.Id,
            Name = patient.Name,
            Email = patient.Email,
            PhoneNumber = patient.PhoneNumber,
            Address = patient.Address,
            ZipCode = patient.ZipCode,
        }).ToList();

        return Ok(new ApiResponse<List<PatientOfTutorResponseDto>>
            { Message = "Utentes encontrados.", Data = patientsResponse });
    }

    /// <summary>
    /// Retrieves a patient by their ID.
    /// </summary>
    /// <param name="id">The ID of the patient to retrieve.</param>
    /// <returns>An IActionResult containing the patient data or error information.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<PatientResponseDto>>> GetPatientById(int id)
    {
        var patient = await _patientService.GetPatientByIdAsync(id);

        if (patient == null)
        {
            return NotFound(new ApiResponse<PatientResponseDto>
                { Success = false, Message = "Não existe nenhum utente com esse id" });
        }


        var patientResponse =
            new PatientResponseDto
            {
                Id = patient.Id,
                Name = patient.Name,
                Email = patient.Email,
                PhoneNumber = patient.PhoneNumber,
                Address = patient.Address,
                ZipCode = patient.ZipCode,
                TutorId = patient.TutorId,
                TutorName = patient.Tutor.Name
            };
        return Ok(new ApiResponse<PatientResponseDto> { Message = "Utente encontrado", Data = patientResponse });
    }

    /// <summary>
    /// Updates an existing patient by ID.
    /// </summary>
    /// <param name="id">The ID of the patient to update.</param>
    /// <param name="patientDto">The updated patient data.</param>
    /// <returns>An IActionResult containing the result of the update operation.</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdatePatientById(int id,
        [FromBody] PatientRequestDto patientDto)
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

        var updated = await _patientService.UpdatePatientAsync(id, patientDto);

        if (!updated)
        {
            return NotFound(
                new ApiResponse<bool> { Success = false, Message = "Não existe nenhum utente com esse id" });
        }

        return Ok(new ApiResponse<bool> { Message = "Os dados do utente foram atualizados" });
    }

    /// <summary>
    /// Deletes a patient by their ID.
    /// </summary>
    /// <param name="id">The ID of the patient to delete.</param>
    /// <returns>An IActionResult containing the result of the delete operation.</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeletePatientById(int id)
    {
        var deleted = await _patientService.DeletePatientAsync(id);
        if (!deleted)
        {
            return NotFound(
                new ApiResponse<bool> { Success = false, Message = "Não existe nenhum utente com esse id" });
        }

        return Ok(new ApiResponse<bool> { Message = "Utente removido" });
    }
}