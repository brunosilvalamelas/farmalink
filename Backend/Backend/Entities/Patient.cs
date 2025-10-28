using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Entities
{
    /// <summary>
    /// Represents a patient within the system.
    /// Inherits common personal information from <see cref="Person"/> 
    /// and includes a collection of medications prescribed to the patient.
    /// </summary>
    public class Patient : User
    {
        [ForeignKey("Tutor")] public int TutorId { get; set; }

        /// <summary>
        /// Street address of the person.
        /// Maximum length of 50 characters.
        /// </summary>
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(50, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
        public string Address { get; set; } = string.Empty;

        public Tutor Tutor { get; set; } = null!;


        /// <summary>
        /// Collection of medications associated with the patient.
        /// Represents the medications currently prescribed or managed for this patient.
        /// </summary>
        public ICollection<Medication> Medications { get; set; } = new List<Medication>();
    }
}