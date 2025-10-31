namespace Backend.Exceptions;

/// <summary>
/// Represents a validation error containing field and message information.
/// </summary>
public class ValidationError
{
    /// <summary>
    /// Gets or sets the field name associated with the validation error.
    /// </summary>
    public string Field { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the error message describing the validation issue.
    /// </summary>
    public string Message { get; set; } = string.Empty;
}
