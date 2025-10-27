namespace Backend.DTOs.response;

/// <summary>
/// Response DTO for patient information.
/// </summary>
public class PatientOfTutorResponseDto
{
    /// <summary>
    /// Unique identifier for the patient.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Full name of the patient.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Email address of the patient.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Phone number of the patient.
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Street address of the patient.
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Postal code of the patient's address.
    /// </summary>
    public string ZipCode { get; set; } = string.Empty;
}