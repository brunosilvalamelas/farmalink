using Backend.Exceptions;

namespace Backend.Services;

/// <summary>
/// Represents the standardized result of a service operation, containing success status, data, and potential errors.
/// </summary>
/// <typeparam name="T">The type of data contained in the result.</typeparam>
public class ServiceResult<T>
{
    /// <summary>
    /// Gets a value indicating whether the operation was successful (Ok or NoContent result type).
    /// </summary>
    public bool Success =>
        ResultType == ServiceResultType.Ok || ResultType == ServiceResultType.NoContent;

    /// <summary>
    /// Gets the type of the service result.
    /// </summary>
    public ServiceResultType ResultType { get; }

    /// <summary>
    /// Gets the message associated with the service result.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets the collection of validation errors, if any occurred.
    /// </summary>
    public IEnumerable<ValidationError> Errors { get; }

    /// <summary>
    /// Gets the data payload of the service result.
    /// </summary>
    public T? Data { get; }

    /// <summary>
    /// Initializes a new instance of the ServiceResult class.
    /// </summary>
    /// <param name="resultType">The type of the result.</param>
    /// <param name="message">The message describing the result.</param>
    /// <param name="data">The data payload.</param>
    /// <param name="errors">The collection of validation errors.</param>
    private ServiceResult(ServiceResultType resultType, string message = "", T? data = default,
        IEnumerable<ValidationError>? errors = null)
    {
        ResultType = resultType;
        Message = message;
        Data = data;
        Errors = errors ?? Enumerable.Empty<ValidationError>();
    }

    /// <summary>
    /// Creates a successful service result with data.
    /// </summary>
    /// <param name="data">The data payload.</param>
    /// <param name="message">The success message.</param>
    /// <returns>A ServiceResult instance representing a successful operation.</returns>
    public static ServiceResult<T> Ok(T? data, string message = "Sucesso") =>
        Create(ServiceResultType.Ok, message, data);

    /// <summary>
    /// Creates a not found service result.
    /// </summary>
    /// <param name="message">The not found message.</param>
    /// <returns>A ServiceResult instance representing a not found resource.</returns>
    public static ServiceResult<T> NotFound(string message = "Recurso não encontrado") =>
        Create(ServiceResultType.NotFound, message);

    /// <summary>
    /// Creates a service result indicating that the resource already exists, with validation errors.
    /// </summary>
    /// <param name="errors">The validation errors.</param>
    /// <param name="message">The message describing the conflict.</param>
    /// <returns>A ServiceResult instance representing a conflict due to existing resource.</returns>
    public static ServiceResult<T> AlreadyExists(IEnumerable<ValidationError> errors,
        string message = "Dados duplicados") =>
        Create(ServiceResultType.AlreadyExists, message, default, errors);

    /// <summary>
    /// Creates a no content service result indicating successful completion without data.
    /// </summary>
    /// <param name="message">The success message.</param>
    /// <returns>A ServiceResult instance representing successful operation without data.</returns>
    public static ServiceResult<T> NoContent(string message = "Sucesso") =>
        Create(ServiceResultType.NoContent, message);

    /// <summary>
    /// Creates a new instance of ServiceResult with the specified parameters.
    /// </summary>
    /// <param name="type">The result type.</param>
    /// <param name="message">The result message.</param>
    /// <param name="data">The data payload.</param>
    /// <param name="errors">The validation errors.</param>
    /// <returns>A new ServiceResult instance.</returns>
    private static ServiceResult<T> Create(ServiceResultType type, string message = "Recurso criado", T? data = default,
        IEnumerable<ValidationError>? errors = null)
        => new(type, message, data, errors);
}
