using Backend.DTOs.response;
using Backend.Entities.Enums;

namespace Backend.Services;

/// <summary>
/// Interface for patient-related service operations.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Retrieves all patients from the database.
    /// </summary>
    /// <returns>A ServiceResult containing the list of patients.</returns>
    Task<AuthResponseDto?> AuthenticateUserAsync(string email, string password);

    Task ValidateDuplicatesAsync<T>(Dictionary<string, string> fieldsToCheck) where T : class;

    string GenerateToken(int userId, UserRole userRole);
}