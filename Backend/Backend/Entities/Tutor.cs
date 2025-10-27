namespace Backend.Entities;

/// <summary>
/// Represents a tutor within the system.
/// Inherits common personal information from <see cref="Person"/>.
/// A tutor can be associated with multiple patients.
/// </summary>
public class Tutor : Person
{
    /// <summary>
    /// Collection of patients associated with the tutor.
    /// Represents the patients under this tutor's care.
    /// </summary>
    public ICollection<Patient> Patients { get; set; } = new List<Patient>();

}
