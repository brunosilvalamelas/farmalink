namespace Backend.Services;

/// <summary>
/// Enumeration representing the possible types of service operation results.
/// </summary>
public enum ServiceResultType
{
    /// <summary>
    /// Indicates a successful operation that returns data.
    /// </summary>
    Ok,

    /// <summary>
    /// Indicates a successful operation without returning data (e.g., updates or deletes).
    /// </summary>
    NoContent,

    /// <summary>
    /// Indicates that the requested resource was not found.
    /// </summary>
    NotFound,

    /// <summary>
    /// Indicates that the resource already exists, typically accompanied by validation errors.
    /// </summary>
    AlreadyExists,
}
