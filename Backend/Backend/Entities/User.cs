using System.ComponentModel.DataAnnotations;
using Backend.Entities.Enums;

namespace Backend.Entities;

public class User
{
    /// <summary>
    /// Unique identifier for the person.
    /// Primary key.
    /// </summary>
    [Key]
    public int Id { get; set; }

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
    /// Hash of the user's password for secure authentication.
    /// </summary>
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [StringLength(100, ErrorMessage = "O campo {0} deve ter no máximo 100 caracteres.")]
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Phone number of the person.
    /// Must follow the Portuguese phone number format (optional +351 prefix).
    /// </summary>
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [StringLength(15, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    [RegularExpression(@"^(?:\+351)? ?[29][0-9]{8}$",
        ErrorMessage = "O campo {0} não é um número de telemóvel português válido.")]
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Postal code of the person's address.
    /// Must follow the Portuguese format (e.g., 1234-567).
    /// Maximum length of 20 characters.
    /// </summary>
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [StringLength(20, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    [RegularExpression(@"^\d{4}-\d{3}$", ErrorMessage = "O campo {0} não é um código postal português válido.")]
    public string ZipCode { get; set; } = string.Empty;

    /// <summary>
    /// The role assigned to the user, determining access levels and permissions.
    /// </summary>
    [Required] public UserRole Role { get; set; }
}
