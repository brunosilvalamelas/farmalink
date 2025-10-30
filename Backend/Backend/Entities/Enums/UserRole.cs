using System.Text.Json.Serialization;

namespace Backend.Entities.Enums;

/// <summary>
/// Defines the possible roles that a user can have within the system.
/// Used for access control and permissions.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserRole
{
    Patient,
    Tutor,
    Employee
}
