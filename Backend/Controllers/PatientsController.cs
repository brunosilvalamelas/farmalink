using Backend.DTOs.request;
using Backend.DTOs.response;
using Backend.Helpers;
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
    private readonly PatientService _patientService;

    /// <summary>
    /// Initializes a new instance of the PatientsController.
    /// </summary>
    /// <param name="patientService">The patient service instance.</param>
    public PatientsController(PatientService patientService)
    {
        _patientService = patientService;
    }

    /// <summary>
    /// Creates a new patient.
    /// </summary>
    /// <param name="patientDto">The patient data to create.</param>
    /// <returns>An IActionResult containing the created patient or error information.</returns>
    [HttpPost]
    public async Task<IActionResult> CreatePatient([FromBody] PatientRequestDto patientDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _patientService.CreatePatientAsync(patientDto);
        var dtoResult = ServiceResultMapper.Map(result, p => new PatientResponseDto
        {
            Id = p.Id,
            Name = p.Name,
            Email = p.Email,
            PhoneNumber = p.PhoneNumber,
            Address = p.Address,
            ZipCode = p.ZipCode
        });

        if (dtoResult.Success && dtoResult.Data != null)
            return CreatedFromServiceResult(dtoResult, nameof(GetPatientById), new { id = dtoResult.Data.Id });


        return FromServiceResult(dtoResult);
    }

    /// <summary>
    /// Retrieves all patients.
    /// </summary>
    /// <returns>An IActionResult containing the list of patients or error information.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllPatients()
    {
        var result = await _patientService.GetAllPatientsAsync();

        var dtoResult = ServiceResultMapper.Map(result, list =>
            list.Select(p => new PatientResponseDto
            {
                Medications = p.Medications,
                Id = p.Id,
                Name = p.Name,
                Email = p.Email,
                PhoneNumber = p.PhoneNumber,
                Address = p.Address,
                ZipCode = p.ZipCode,
            }).ToList()
        );
        return FromServiceResult(dtoResult);
    }

    /// <summary>
    /// Retrieves a patient by their ID.
    /// </summary>
    /// <param name="id">The ID of the patient to retrieve.</param>
    /// <returns>An IActionResult containing the patient data or error information.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPatientById(int id)
    {
        var result = await _patientService.GetPatientByIdAsync(id);
        var dtoResult = ServiceResultMapper.Map(result,
            p => new PatientResponseDto
            {
                Medications = p.Medications,
                Id = p.Id,
                Name = p.Name,
                Email = p.Email,
                PhoneNumber = p.PhoneNumber,
                Address = p.Address,
                ZipCode = p.ZipCode,
            }
        );
        return FromServiceResult(dtoResult);
    }

    /// <summary>
    /// Updates an existing patient by ID.
    /// </summary>
    /// <param name="id">The ID of the patient to update.</param>
    /// <param name="patientDto">The updated patient data.</param>
    /// <returns>An IActionResult containing the result of the update operation.</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePatientById(int id, [FromBody] PatientRequestDto patientDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _patientService.UpdatePatientAsync(id, patientDto);

        return FromServiceResult(result);
    }

    /// <summary>
    /// Deletes a patient by their ID.
    /// </summary>
    /// <param name="id">The ID of the patient to delete.</param>
    /// <returns>An IActionResult containing the result of the delete operation.</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePatientById(int id)
    {
        var result = await _patientService.DeletePatientAsync(id);
        return FromServiceResult(result);
    }
}