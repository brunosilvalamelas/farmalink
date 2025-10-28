using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.request;

/// <summary>
/// DTO for user login credentials.
/// </summary>
public class LoginRequestDto
{
    /// <summary>
    /// Email address of the person.
    /// Must be a valid email format.
    /// Maximum length of 50 characters.
    /// </summary>
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [EmailAddress(ErrorMessage = "O campo {0} não é um endereço de e-mail válido.")]
    [StringLength(50, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Plain text password for authentication.
    /// </summary>
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public string Password { get; set; } = string.Empty;
}
