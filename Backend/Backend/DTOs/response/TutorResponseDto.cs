namespace Backend.DTOs.response;

/// <summary>
/// Response DTO for tutor information.
/// </summary>
public class TutorResponseDto
{
    /// <summary>
    /// Unique identifier for the tutor.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Full name of the tutor.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Email address of the tutor.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Phone number of the tutor.
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Street address of the tutor.
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Postal code of the tutor's address.
    /// </summary>
    public string ZipCode { get; set; } = string.Empty;
}