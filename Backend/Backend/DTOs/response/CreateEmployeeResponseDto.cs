using Backend.Entities.Enums;

namespace Backend.DTOs.response;

public class CreateEmployeeResponseDto
{
    /// <summary>
    /// JWT authentication token for the created tutor.
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Full name of the created tutor.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The role assigned to the user.
    /// </summary>
    public UserRole Role { get; set; }
}