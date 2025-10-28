using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.request;

/// <summary>
/// Request DTO for updating tutor information.
/// </summary>
public class UpdateTutorRequestDto
{
    /// <summary>
    /// Full name of the person.
    /// Maximum length of 20 characters.
    /// </summary>
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [StringLength(20, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string Name { get; set; } = string.Empty;


    /// <summary>
    /// Street address of the person.
    /// Maximum length of 50 characters.
    /// </summary>
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [StringLength(50, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Postal code of the person's address.
    /// Must follow the Portuguese format (e.g., 1234-567).
    /// Maximum length of 20 characters.
    /// </summary>
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [StringLength(20, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    [RegularExpression(@"^\d{4}-\d{3}$", ErrorMessage = "O campo {0} não é um código postal português válido.")]
    public string ZipCode { get; set; } = string.Empty;
}
