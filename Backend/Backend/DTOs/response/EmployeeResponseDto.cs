namespace Backend.DTOs.response;

/// <summary>
/// Data Transfer Object for employee response data.
/// </summary>
public class EmployeeResponseDto
{
    /// <summary>
    /// Unique identifier for the employee.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Full name of the employee.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Email address of the employee.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Phone number of the employee.
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;
    /// <summary>
    /// Delivery Location of the employee.
    /// </summary>
    public string DeliveryLocation { get; set; } = string.Empty;
}
