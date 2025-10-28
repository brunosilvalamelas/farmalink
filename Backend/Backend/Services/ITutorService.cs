using Backend.DTOs.request;
using Backend.Entities;

namespace Backend.Services;

/// <summary>
/// Interface for tutor-related service operations.
/// </summary>
public interface ITutorService
{
    /// <summary>
    /// Creates a new tutor in the database.
    /// </summary>
    /// <param name="tutorDto">The data to create a new tutor.</param>
    /// <returns>A ServiceResult containing the created tutor or validation errors.</returns>
    Task<(Tutor tutor, string token)> CreateTutorAsync(CreateTutorRequestDto createTutorDto);

    /// <summary>
    /// Retrieves all tutors from the database.
    /// </summary>
    /// <returns>A ServiceResult containing the list of tutors.</returns>
    Task<List<Tutor>> GetAllTutorsAsync();

    /// <summary>
    /// Retrieves a tutor by their ID.
    /// </summary>
    /// <param name="id">The ID of the tutor to retrieve.</param>
    /// <returns>A ServiceResult containing the tutor or a not found result.</returns>
    Task<Tutor?> GetTutorByIdAsync(int id);

    /// <summary>
    /// Updates an existing tutor with new data.
    /// </summary>
    /// <param name="id">The ID of the tutor to update.</param>
    /// <param name="updateTutorDto">The updated tutor data.</param>
    /// <returns>A ServiceResult indicating the success of the update operation.</returns>
    Task<bool> UpdateTutorAsync(int id, UpdateTutorRequestDto updateTutorDto);

    /// <summary>
    /// Deletes a tutor by their ID.
    /// </summary>
    /// <param name="id">The ID of the tutor to delete.</param>
    /// <returns>A ServiceResult indicating the success of the delete operation.</returns>
    Task<bool> DeleteTutorAsync(int id);
}