using Backend.Controllers;

namespace Backend.Middleware;

/// <summary>
/// Middleware for handling exceptions globally in the API.
/// Catches any unhandled exceptions and returns a standardized ApiResponse.
/// </summary>
public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Initializes a new instance of the ErrorHandlerMiddleware.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    public ErrorHandlerMiddleware(RequestDelegate next) => _next = next;

    /// <summary>
    /// Processes an HTTP request, attempting to execute the next middleware and handling any exceptions.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        // Try to execute the next middleware in the pipeline
        try
        {
            await _next(context);
        }
        // Catch any unhandled exception
        catch (Exception e)
        {
            // Set the HTTP response to 500 Internal Server Error
            context.Response.StatusCode = 500;
            // Set the content type to JSON
            context.Response.ContentType = "application/json";

            // Create a standardized error response using ApiResponse
            var apiResponse = new ApiResponse<object>
            {
                Success = false,
                Message = e.Message,
                Data = null,
                Errors = null
            };

            // Write the ApiResponse as JSON to the response
            await context.Response.WriteAsJsonAsync(apiResponse);
        }
    }
}