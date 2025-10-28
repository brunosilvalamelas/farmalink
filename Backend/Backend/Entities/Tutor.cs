using System.ComponentModel.DataAnnotations;

namespace Backend.Entities;

/// <summary>
/// Represents a tutor within the system.
/// Inherits common personal information from <see cref="Person"/>.
/// A tutor can be associated with multiple patients.
/// </summary>
public class Tutor : User
{
    /// <summary>
    /// Street address of the person.
    /// Maximum length of 50 characters.
    /// </summary>
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [StringLength(50, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string Address { get; set; } = string.Empty;


    /// <summary>
    /// Collection of patients associated with the tutor.
    /// Represents the patients under this tutor's care.
    /// </summary>
    public ICollection<Patient> Patients { get; set; } = new List<Patient>();
}