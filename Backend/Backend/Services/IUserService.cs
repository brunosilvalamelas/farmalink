using Backend.DTOs.response;
using Backend.Entities.Enums;

namespace Backend.Services;

/// <summary>
/// Interface for user-related service operations.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Authenticates a user by email and password.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="password">The user's plain text password.</param>
    /// <returns>An AuthResponseDto containing authentication details, or null if authentication failed.</returns>
    Task<AuthResponseDto?> AuthenticateUserAsync(string email, string password);

    /// <summary>
    /// Validates if the provided fields contain unique values across the specified entity type.
    /// Throws a ValidationException if duplicates are found.
    /// </summary>
    /// <typeparam name="T">The entity type to check against (e.g., User).</typeparam>
    /// <param name="fieldsToCheck">Dictionary of field names and values to validate for uniqueness.</param>
    Task ValidateDuplicatesAsync<T>(Dictionary<string, string> fieldsToCheck) where T : class;

    /// <summary>
    /// Generates a JWT token for the authenticated user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="userRole">The role of the user for authorization claims.</param>
    /// <returns>A JWT token string.</returns>
    string GenerateToken(int userId, UserRole userRole);
}
