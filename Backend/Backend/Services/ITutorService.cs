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
    /// <param name="createTutorDto">The data to create a new tutor.</param>
    /// <returns>A tuple containing the created tutor and the authentication token.</returns>
    Task<(Tutor tutor, string token)> CreateTutorAsync(CreateTutorRequestDto createTutorDto);

    /// <summary>
    /// Retrieves all tutors from the database.
    /// </summary>
    /// <returns>The list of tutors.</returns>
    Task<List<Tutor>> GetAllTutorsAsync();

    /// <summary>
    /// Retrieves a tutor by their ID.
    /// </summary>
    /// <param name="id">The ID of the tutor to retrieve.</param>
    /// <returns>The tutor if found, or null otherwise.</returns>
    Task<Tutor?> GetTutorByIdAsync(int id);

    /// <summary>
    /// Updates an existing tutor with new data.
    /// </summary>
    /// <param name="id">The ID of the tutor to update.</param>
    /// <param name="updateTutorDto">The updated tutor data.</param>
    /// <returns>True if the update was successful, false otherwise.</returns>
    Task<bool> UpdateTutorAsync(int id, UpdateTutorRequestDto updateTutorDto);

    /// <summary>
    /// Deletes a tutor by their ID.
    /// </summary>
    /// <param name="id">The ID of the tutor to delete.</param>
    /// <returns>True if the deletion was successful, false otherwise.</returns>
    Task<bool> DeleteTutorAsync(int id);
}
