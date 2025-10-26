namespace Backend.Services;

/// <summary>
/// Enumeration representing the possible types of service operation results.
/// </summary>
public enum ServiceResultType
{
    /// <summary>
    /// Indicates a successful operation.
    /// </summary>
    Ok,

    /// <summary>
    /// Indicates that the required item or data was not found and the operation could not continue without it.
    /// </summary>
    NotFound,

    /// <summary>
    /// Indicates validation errors in the provided data.
    /// </summary>
    ValidationError,
}