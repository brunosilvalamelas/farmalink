using Backend.DTOs.request;
using Backend.Entities;

namespace Backend.Services;

/// <summary>
/// Interface for patient-related service operations.
/// </summary>
public interface IPatientService
{
    /// <summary>
    /// Creates a new patient associated with the specified tutor.
    /// </summary>
    /// <param name="loggedInTutorId">The ID of the tutor creating the patient.</param>
    /// <param name="createPatientDto">The data transfer object containing patient creation details.</param>
    /// <returns>The created patient entity or null if tutor doesn't exist.</returns>
    Task<Patient?> CreatePatientAsync(int loggedInTutorId, CreatePatientRequestDto createPatientDto);
    /// <summary>
    /// Retrieves all patients from the database.
    /// </summary>
    /// <returns>The list of patients.</returns>
    Task<List<Patient>> GetAllPatientsAsync();

    /// <summary>
    /// Retrieves a patient by their ID.
    /// </summary>
    /// <param name="id">The ID of the patient to retrieve.</param>
    /// <returns>The patient if found, or null otherwise.</returns>
    Task<Patient?> GetPatientByIdAsync(int id);

    /// <summary>
    /// Retrieves patients by their tutor's ID.
    /// </summary>
    /// <param name="tutorId">The ID of the tutor to retrieve patients for.</param>
    /// <returns>The list of patients if the tutor exists, or null otherwise.</returns>
    Task<List<Patient>?> GetPatientsByTutorIdAsync(int tutorId);

    /// <summary>
    /// Updates an existing patient with new data.
    /// </summary>
    /// <param name="id">The ID of the patient to update.</param>
    /// <param name="updatePatientDto">The updated patient data.</param>
    /// <returns>True if the update was successful, false otherwise.</returns>
    Task<bool> UpdatePatientAsync(int id, UpdatePatientRequestDto updatePatientDto);

    /// <summary>
    /// Deletes a patient by their ID.
    /// </summary>
    /// <param name="id">The ID of the patient to delete.</param>
    /// <returns>True if the deletion was successful, false otherwise.</returns>
    Task<bool> DeletePatientAsync(int id);
}
