using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.request
{
    public class CreateIntakeInstructionRequestDto
    {
        [Required]
        public int MedicationId { get; set; }

        [Required]
        [Range(0.01, 1000.00)]
        public decimal DosePerIntake { get; set; }

        [Required]
        [StringLength(20)]
        public string DosageRegime { get; set; } = null!;

        [Required]
        public TimeSpan Time { get; set; }

        public int? RoutineId { get; set; } // Optional
    }

    public class UpdateIntakeInstructionRequestDto
    {
        [Required]
        [Range(0.01, 1000.00)]
        public decimal DosePerIntake { get; set; }

        [Required]
        [StringLength(20)]
        public string DosageRegime { get; set; } = null!;

        [Required]
        public TimeSpan Time { get; set; }

        public int? RoutineId { get; set; }
    }
}