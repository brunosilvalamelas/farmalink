using System.Text.Json.Serialization;

namespace Backend.Entities.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserRole
{
    Patient,
    Tutor,
    Employee
}