namespace Backend.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

{
    /// <summary>
    /// Represents intake instructions for medications, optionally associated with routines.
    /// Core entity for US2 - Medication intake instructions with routine association.
    /// </summary>
    public class IntakeInstruction
    {
        /// <summary>
        /// Unique identifier for the intake instruction.
        /// Primary key.
        /// </summary>
        [Key]
        public int IntakeInstructionId { get; set; }

        /// <summary>
        /// Foreign key to the routine (optional association).
        /// Nullable for instructions not associated with routines.
        /// </summary>
        [ForeignKey("Routine")]
        public int? RoutineId { get; set; }

        /// <summary>
        /// Foreign key to the medication.
        /// </summary>
        [ForeignKey("Medication")]
        public int MedicationId { get; set; }

        /// <summary>
        /// Dose quantity to be taken per intake.
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal DosePerIntake { get; set; }

        /// <summary>
        /// Dosage regime type (e.g., "Daily", "Weekly", "As needed").
        /// Maximum length 20 characters.
        /// </summary>
        [Required]
        [StringLength(20)]
        public string DosageRegime { get; set; } = null!;

        /// <summary>
        /// Time of day for medication intake.
        /// </summary>
        [Required]
        public TimeSpan Time { get; set; }

        /// <summary>
        /// Navigation property to the routine (optional).
        /// </summary>
        [JsonIgnore]
        public Routine? Routine { get; set; }

        /// <summary>
        /// Navigation property to the medication.
        /// </summary>
        [JsonIgnore]
        public Medication? Medication { get; set; }
    }
}