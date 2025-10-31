namespace Backend.Exceptions;

/// <summary>
/// Represents an exception that contains multiple validation errors.
/// </summary>
public class ValidationException : Exception
{
    /// <summary>
    /// Gets the collection of validation errors.
    /// </summary>
    public IEnumerable<ValidationError> Errors { get; }

    /// <summary>
    /// Initializes a new instance of the ValidationException class with the specified errors.
    /// </summary>
    /// <param name="errors">The collection of validation errors.</param>
    public ValidationException(IEnumerable<ValidationError> errors)
        : base("Errors de validação.")
    {
        Errors = errors;
    }
}