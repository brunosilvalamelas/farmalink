using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.request;

/// <summary>
/// Data Transfer Object for creating a new employee.
/// </summary>
public class CreateEmployeeRequestDto
{
     /// <summary>
    /// Full name of the person.
    /// Maximum length of 20 characters.
    /// </summary>
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [StringLength(20, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string Name { get; set; } = string.Empty;

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
    /// Plain text password for the user account.
    /// Minimum length of 8 characters, maximum 64.
    /// </summary>
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [StringLength(64, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    [MinLength(8, ErrorMessage = "O campo {0} deve ter no mínimo {1} caracteres.")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Phone number of the person.
    /// Must follow the Portuguese phone number format (optional +351 prefix).
    /// </summary>
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [RegularExpression(@"^(?:\+351)? ?[29][0-9]{8}$",
        ErrorMessage = "O campo {0} não é um número de telemóvel português válido.")]
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Delivery Location of the employee.
    /// Maximum length of 30 characters.
    /// </summary>
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [StringLength(30, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string DeliveryLocation { get; set; } = string.Empty;
}
