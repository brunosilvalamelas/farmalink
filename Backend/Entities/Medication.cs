using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Backend.Entities
{
    /// <summary>
    /// Represents a medication belonging to a patient, with stock and dosage information.
    /// </summary>
    public class Medication
    {
        /// <summary>
        /// Unique identifier for the medication.
        /// Primary key.
        /// </summary>
        [Key]
        public int MedicationId { get; set; }

        /// <summary>
        /// Foreign key to the patient who owns this medication.
        /// </summary>
        [ForeignKey("Patient")]
        public int PatientId { get; set; }

        /// <summary>
        /// Name of the medication.
        /// Maximum length 50 characters.
        /// </summary>
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Quantity available in stock for this patient.
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal QuantityOnHand { get; set; }

        /// <summary>
        /// Quantity per unit of the medication.
        /// </summary>
        [Required]
        public int QuantityPerUnit { get; set; }

        /// <summary>
        /// Threshold for low stock alert.
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal LowStockThreshold { get; set; }

        /// <summary>
        /// Navigation property to the patient.
        /// </summary>
        [JsonIgnore]
        public Patient? Patient { get; set; }
    }
}