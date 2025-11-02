using System.ComponentModel.DataAnnotations;

namespace Backend.Entities;

/// <summary>
/// Represents an employee within the system.
/// Inherits common personal information from <see cref="User"/>.
/// </summary>
public class Employee : User
{
    /// <summary>
    /// Gets or sets the delivery location for the employee.
    /// </summary>
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [StringLength(30, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string DeliveryLocation { get; set; } = string.Empty;
}
