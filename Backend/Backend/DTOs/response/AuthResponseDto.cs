namespace Backend.DTOs.response;

/// <summary>
/// DTO for authentication response containing login details and JWT token.
/// </summary>
public class AuthResponseDto
{
    /// <summary>
    /// Initializes a new instance of the AuthResponseDto class.
    /// </summary>
    /// <param name="loginResponse">The login response details for the user.</param>
    /// <param name="token">The JWT authentication token.</param>
    public AuthResponseDto(LoginResponseDto loginResponse, string token)
    {
        LoginResponse = loginResponse;
        Token = token;
    }

    /// <summary>
    /// The login response details for the authenticated user.
    /// </summary>
    public LoginResponseDto LoginResponse { get; set; }

    /// <summary>
    /// The JWT authentication token.
    /// </summary>
    public string Token { get; set; }
}
