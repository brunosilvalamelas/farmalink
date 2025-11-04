using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Backend.Entities
{
    /// <summary>
    /// Represents a routine item for a patient, used to associate medication intake times.
    /// </summary>
    public class Routine
    {
        /// <summary>
        /// Unique identifier for the routine.
        /// Primary key.
        /// </summary>
        [Key]
        public int RoutineId { get; set; }

        /// <summary>
        /// Foreign key to the patient.
        /// </summary>
        [ForeignKey("Patient")]
        public int PatientId { get; set; }

        /// <summary>
        /// Description of the routine item.
        /// Maximum length 256 characters.
        /// </summary>
        [Required]
        [StringLength(256)]
        public string Description { get; set; } = null!;

        /// <summary>
        /// Time when the routine occurs.
        /// </summary>
        [Required]
        public TimeSpan Time { get; set; }

        /// <summary>
        /// Duration of the routine activity.
        /// </summary>
        [Required]
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Navigation property to the patient.
        /// </summary>
        [JsonIgnore]
        public Patient? Patient { get; set; }

        /// <summary>
        /// Navigation property to intake instructions associated with this routine.
        /// </summary>
        [JsonIgnore]
        public ICollection<IntakeInstruction> IntakeInstructions { get; set; } = new List<IntakeInstruction>();
    }
}