using Backend.DTOs.request;
using Backend.Entities;

namespace Backend.Services
{
    public interface IIntakeInstructionService
    {
        Task<IntakeInstruction?> GetByIdAsync(int id);
        Task<List<IntakeInstruction>> GetByMedicationIdAsync(int medicationId);
        Task<List<IntakeInstruction>> GetByRoutineIdAsync(int routineId);
        Task<List<IntakeInstruction>> GetByPatientIdAsync(int patientId);
        Task<IntakeInstruction> CreateAsync(CreateIntakeInstructionRequestDto request);
        Task<IntakeInstruction?> UpdateAsync(int id, UpdateIntakeInstructionRequestDto request);
        Task<bool> DeleteAsync(int id);
    }
}