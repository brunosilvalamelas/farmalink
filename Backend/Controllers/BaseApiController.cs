using Backend.Exceptions;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

/// <summary>
/// Base class for API controllers providing common functionality for handling service results.
/// </summary>
public abstract class BaseApiController : ControllerBase
{
    /// <summary>
    /// Converts a ServiceResult to the appropriate IActionResult based on the result type.
    /// </summary>
    /// <typeparam name="T">The type of data contained in the service result.</typeparam>
    /// <param name="result">The service result to convert to an action result.</param>
    /// <returns>An IActionResult representing the HTTP response.</returns>
    protected IActionResult FromServiceResult<T>(ServiceResult<T> result)
    {
        var response = new ApiResponse<T>
        {
            Success = result.Success,
            Message = result.Message,
            Data = result.Data,
            Errors = result.Errors
        };

        return result.ResultType switch
        {
            ServiceResultType.Ok => Ok(response),
            ServiceResultType.NotFound => NotFound(response),
            ServiceResultType.ValidationError => BadRequest(response),
            _ => StatusCode(500, new ApiResponse<T>
            {
                Success = false,
                Message = "Unexpected error",
                Errors = new List<ValidationError>()
            })
        };
    }
}
