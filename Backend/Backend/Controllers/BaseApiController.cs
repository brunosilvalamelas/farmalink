using Backend.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

/// <summary>
/// Base class for API controllers providing common functionality.
/// </summary>
public abstract class BaseApiController : ControllerBase
{
    /// <summary>
    /// Extracts validation errors from ModelState into a list of ValidationError objects.
    /// </summary>
    /// <returns>A list of validation errors.</returns>
    protected List<ValidationError> GetModelStateErrors()
    {
        return ModelState
            .Where(ms => ms.Value?.Errors.Any() == true)
            .SelectMany(kvp => kvp.Value!.Errors.Select(e => new ValidationError
            {
                Field = kvp.Key.ToLower(),
                Message = e.ErrorMessage ?? string.Empty
            }))
            .ToList();
    }
}