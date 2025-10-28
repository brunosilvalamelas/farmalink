using Backend.Entities.Enums;

namespace Backend.DTOs.response;

/// <summary>
/// DTO for the response after successful user login, containing authentication token and user info.
/// </summary>
public class LoginResponseDto
{
    /// <summary>
    /// JWT authentication token for the logged-in user.
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Full name of the logged-in user.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The role assigned to the user.
    /// </summary>
    public UserRole Role { get; set; }
}
