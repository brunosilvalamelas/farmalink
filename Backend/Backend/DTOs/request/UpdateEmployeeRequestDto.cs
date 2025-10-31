using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.request;

public class UpdateEmployeeRequestDto
{
    /// <summary>
    /// Full name of the person.
    /// Maximum length of 20 characters.
    /// </summary>
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [StringLength(20, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string Name { get; set; } = string.Empty;


    /// <summary>
    /// Delivery Location of the employee.
    /// Maximum length of 50 characters.
    /// </summary>
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [StringLength(30, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string DeliveryLocation { get; set; } = string.Empty;
    
}