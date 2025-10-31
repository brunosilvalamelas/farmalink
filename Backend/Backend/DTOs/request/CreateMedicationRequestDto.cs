using System.ComponentModel.DataAnnotations;

public class CreateMedicationRequestDto
{
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [Range(1, int.MaxValue, ErrorMessage = "O campo {0} deve ser um valor positivo.")]
    public int PatientId { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [StringLength(50, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [Range(0.01, 999.99, ErrorMessage = "O campo {0} deve estar entre {1} e {2}.")]
    public decimal QuantityOnHand { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [Range(1, int.MaxValue, ErrorMessage = "O campo {0} deve ser um valor positivo.")]
    public int QuantityPerUnit { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [Range(0.01, 999.99, ErrorMessage = "O campo {0} deve estar entre {1} e {2}.")]
    public decimal LowStockThreshold { get; set; }
}