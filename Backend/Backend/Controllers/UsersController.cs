using Backend.DTOs.request;
using Backend.DTOs.response;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

/// <summary>
/// API controller for managing user authentication operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UsersController : BaseApiController
{
    private readonly IUserService _userService;

    /// <summary>
    /// Initializes a new instance of the UsersController.
    /// </summary>
    /// <param name="userService">The user service instance.</param>
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Authenticates a user with email and password, returning a JWT token if successful.
    /// </summary>
    /// <param name="loginRequest">The login credentials.</param>
    /// <returns>An IActionResult containing the authentication result and token.</returns>
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login([FromBody] LoginRequestDto loginRequest)
    {
        if (!ModelState.IsValid)
        {
            var errors = GetModelStateErrors();
            return BadRequest(new ApiResponse<LoginResponseDto>
            {
                Success = false,
                Message = "Erros de validação",
                Errors = errors
            });
        }

        var authResult = await _userService.AuthenticateUserAsync(loginRequest.Email, loginRequest.Password);
        if (authResult == null)
        {
            return Unauthorized(new ApiResponse<LoginResponseDto>
                { Success = false, Message = "Email ou password inválido" });
        }

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddMinutes(120)
        };

        Response.Cookies.Append("access_token", authResult.Token, cookieOptions);


        return Ok(new ApiResponse<LoginResponseDto>
            { Message = "Autenticação efetuada", Data = authResult.LoginResponse });
    }
}