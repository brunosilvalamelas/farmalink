using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.request;

/// <summary>
/// Request DTO for updating medication information.
/// </summary>
public class UpdateMedicationRequestDto
{
    /// <summary>
    /// Foreign key to the patient who owns this medication.
    /// </summary>
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [Range(1, int.MaxValue, ErrorMessage = "O campo {0} deve ser um valor positivo.")]
    public int PatientId { get; set; }

    /// <summary>
    /// Name of the medication.
    /// Maximum length 50 characters.
    /// </summary>
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [StringLength(50, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Quantity available in stock for this patient.
    /// Must be a positive value with 2 decimal places.
    /// </summary>
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [Range(0.01, 999.99, ErrorMessage = "O campo {0} deve estar entre {1} e {2}.")]
    public decimal QuantityOnHand { get; set; }

    /// <summary>
    /// Quantity per unit of the medication.
    /// Must be a positive integer.
    /// </summary>
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [Range(1, int.MaxValue, ErrorMessage = "O campo {0} deve ser um valor positivo.")]
    public int QuantityPerUnit { get; set; }

    /// <summary>
    /// Threshold for low stock alert.
    /// Must be a positive value with 2 decimal places.
    /// </summary>
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [Range(0.01, 999.99, ErrorMessage = "O campo {0} deve estar entre {1} e {2}.")]
    public decimal LowStockThreshold { get; set; }
}