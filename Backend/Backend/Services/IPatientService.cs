using Backend.DTOs.request;
using Backend.Entities;

namespace Backend.Services;

/// <summary>
/// Interface for patient-related service operations.
/// </summary>
public interface IPatientService
{
    /// <summary>
    /// Creates a new patient in the database.
    /// </summary>
    /// <param name="tutorId">The ID of the tutor for the new patient.</param>
    /// <param name="patientDto">The patient data to create.</param>
    /// <returns>The created patient or null if the tutor does not exist.</returns>
    Task<Patient?> CreatePatientAsync(int tutorId, PatientRequestDto patientDto);

    /// <summary>
    /// Retrieves all patients from the database.
    /// </summary>
    /// <returns>A ServiceResult containing the list of patients.</returns>
    Task<List<Patient>> GetAllPatientsAsync();

    /// <summary>
    /// Retrieves a patient by their ID.
    /// </summary>
    /// <param name="id">The ID of the patient to retrieve.</param>
    /// <returns>A ServiceResult containing the patient or a not found result.</returns>
    Task<Patient?> GetPatientByIdAsync(int id);

    /// <summary>
    /// Retrieves patients by their tutor's ID.
    /// </summary>
    /// <param name="tutorId">The ID of the tutor to retrieve patients for.</param>
    /// <returns>A list of patients or null if the tutor does not exist.</returns>
    Task<List<Patient>?> GetPatientsByTutorIdAsync(int tutorId);

    /// <summary>
    /// Updates an existing patient with new data.
    /// </summary>
    /// <param name="id">The ID of the patient to update.</param>
    /// <param name="patientDto">The updated patient data.</param>
    /// <returns>A ServiceResult indicating the success of the update operation.</returns>
    Task<bool> UpdatePatientAsync(int id, PatientRequestDto patientDto);

    /// <summary>
    /// Deletes a patient by their ID.
    /// </summary>
    /// <param name="id">The ID of the patient to delete.</param>
    /// <returns>A ServiceResult indicating the success of the delete operation.</returns>
    Task<bool> DeletePatientAsync(int id);
}
