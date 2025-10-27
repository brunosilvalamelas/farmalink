using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Entities
{
    /// <summary>
    /// Represents a patient within the system.
    /// Inherits common personal information from <see cref="Person"/> 
    /// and includes a collection of medications prescribed to the patient.
    /// </summary>
    public class Patient : Person
    {
        /// <summary>
        /// Collection of medications associated with the patient.
        /// Represents the medications currently prescribed or managed for this patient.
        /// </summary>
        public ICollection<Medication> Medications { get; set; } = new List<Medication>();

        [ForeignKey("Tutor")] public int TutorId { get; set; }

        public Tutor Tutor { get; set; } = null!;

        public override string ToString()
        {
            return
                $"Patient(Id={Id}, Name={Name}, Email={Email}, Phone={PhoneNumber}, Medications={Medications.Count})";
        }
    }
}