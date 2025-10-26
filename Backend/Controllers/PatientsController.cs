using Backend.Entities;
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
    /// <param name="newPatient">The patient data to create.</param>
    /// <returns>An IActionResult containing the created patient or error information.</returns>
    [HttpPost]
    public async Task<IActionResult> CreatePatient([FromBody] Patient newPatient)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _patientService.CreatePatient(newPatient);

        if (result.Success && result.Data != null)
            return CreatedFromServiceResult(result, nameof(GetPatientById), new { id = result.Data.Id });

        return FromServiceResult(result);
    }

    /// <summary>
    /// Retrieves all patients.
    /// </summary>
    /// <returns>An IActionResult containing the list of patients or error information.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllPatients()
    {
        var result = await _patientService.GetAllPatients();
        return FromServiceResult(result);
    }

    /// <summary>
    /// Retrieves a patient by their ID.
    /// </summary>
    /// <param name="id">The ID of the patient to retrieve.</param>
    /// <returns>An IActionResult containing the patient data or error information.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPatientById(int id)
    {
        var result = await _patientService.GetPatientById(id);
        return FromServiceResult(result);
    }

    /// <summary>
    /// Updates an existing patient by ID.
    /// </summary>
    /// <param name="id">The ID of the patient to update.</param>
    /// <param name="updatedPatient">The updated patient data.</param>
    /// <returns>An IActionResult containing the result of the update operation.</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePatientById(int id, [FromBody] Patient updatedPatient)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _patientService.UpdatePatient(id, updatedPatient);
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
        var result = await _patientService.DeletePatient(id);
        return FromServiceResult(result);
    }
}