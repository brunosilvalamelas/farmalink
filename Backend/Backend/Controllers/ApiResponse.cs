using Backend.Exceptions;

namespace Backend.Controllers;

/// <summary>
/// Represents a standardized response structure for API controller actions.
/// </summary>
/// <typeparam name="T">The type of the data payload in the response.</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// Gets or sets a value indicating whether the operation was successful.
    /// </summary>
    public bool Success { get; set; } = true;

    /// <summary>
    /// Gets or sets the message associated with the response.
    /// </summary>
    public string Message { get; set; } = "Sucesso";

    /// <summary>
    /// Gets or sets the data payload of the response.
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Gets or sets the collection of validation errors, if any.
    /// </summary>
    public IEnumerable<ValidationError> Errors { get; set; } = new List<ValidationError>();
}