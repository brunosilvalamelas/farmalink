namespace Backend.DTOs.response
{
    public class IntakeInstructionResponseDto
    {
        public int IntakeInstructionId { get; set; }
        public int MedicationId { get; set; }
        public string MedicationName { get; set; } = null!;
        public decimal DosePerIntake { get; set; }
        public string DosageRegime { get; set; } = null!;
        public TimeSpan Time { get; set; }
        public int? RoutineId { get; set; }
        
        public string? RoutineDescription { get; set; } 
        
        public TimeSpan? RoutineTime { get; set; }
    }
}