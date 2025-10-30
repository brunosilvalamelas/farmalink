using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.Data;
using Backend.DTOs.response;
using Backend.Entities;
using Backend.Entities.Enums;
using Backend.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Services;

/// <summary>
/// Service class for handling user authentication, token generation, and validation operations.
/// </summary>
public class UserService : IUserService
{
    private readonly IConfiguration _configuration;
    private readonly DataContext _context;

    public UserService(IConfiguration configuration, DataContext context)
    {
        _configuration = configuration;
        _context = context;
    }

    /// <summary>
    /// Authenticates a user by email and password, returning login data if successful.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="password">The user's plain text password.</param>
    /// <returns>A LoginResponseDto with token and user info, or null if authentication fails.</returns>
    public async Task<AuthResponseDto?> AuthenticateUserAsync(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            return null;
        }

        var isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        if (!isValid)
        {
            return null;
        }

        var token = GenerateToken(user.Id, user.Role);

        var authResponse = new AuthResponseDto(new LoginResponseDto
        {
            Name = user.Name,
            Role = user.Role
        }, token);
        
        return authResponse;
    }

    /// <summary>
    /// Generates a JWT token for the specified user with their role.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="userRole">The role of the user.</param>
    /// <returns>A JWT token string.</returns>
    public string GenerateToken(int userId, UserRole userRole)
    {
        var securityKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, userRole.ToString())
            },
            expires: DateTime.UtcNow.AddMinutes(120),
            signingCredentials:
            credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Validates if the given fields already exist in the specified entity type, throwing an exception if duplicates are found.
    /// </summary>
    /// <typeparam name="T">The entity type to check for duplicates.</typeparam>
    /// <param name="fieldsToCheck">Dictionary of field names and values to validate.</param>
    /// <exception cref="ValidationException">Thrown when duplicate fields are detected.</exception>
    public async Task ValidateDuplicatesAsync<T>(Dictionary<string, string> fieldsToCheck) where T : class
    {
        var errors = new List<ValidationError>();

        foreach (var kvp in fieldsToCheck)
        {
            var fieldName = kvp.Key;
            var fieldValue = kvp.Value;

            bool exists = false;

            if (typeof(T) == typeof(User))
            {
                switch (fieldName)
                {
                    case "Email":
                        exists = await _context.Users.AnyAsync(u => u.Email == fieldValue);
                        break;
                    case "PhoneNumber":
                        exists = await _context.Users.AnyAsync(u => u.PhoneNumber == fieldValue);
                        break;
                }
            }

            if (exists)
                errors.Add(new ValidationError
                    { Field = fieldName, Message = $"Já existe um utilizador com esse {fieldName.ToLower()}." });
        }

        if (errors.Count > 0)
            throw new ValidationException(errors);
    }
}