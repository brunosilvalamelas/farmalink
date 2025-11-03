using Backend.Data;
using Backend.DTOs.request;
using Backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{

    public class IntakeInstructionService : IIntakeInstructionService
    {
        private readonly DataContext _context;
        public IntakeInstructionService(DataContext context)
        {
            _context = context;
        }
        
        public async Task<IntakeInstruction?> GetByIdAsync(int id)
        {
            return await _context.IntakeInstructions
                .Include(i => i.Medication)
                .Include(i => i.Routine)
                .FirstOrDefaultAsync(i => i.IntakeInstructionId == id);
        }
        
        public async Task<List<IntakeInstruction>> GetByMedicationIdAsync(int medicationId)
        {
            return await _context.IntakeInstructions
                .Include(i => i.Medication)
                .Include(i => i.Routine)
                .Where(i => i.MedicationId == medicationId)
                .ToListAsync();
        }
        
        public async Task<List<IntakeInstruction>> GetByRoutineIdAsync(int routineId)
        {
            return await _context.IntakeInstructions
                .Include(i => i.Medication)
                .Include(i => i.Routine)
                .Where(i => i.RoutineId == routineId)
                .ToListAsync();
        }
        
        public async Task<List<IntakeInstruction>> GetByPatientIdAsync(int patientId)
        {
            return await _context.IntakeInstructions
                .Include(i => i.Medication)
                .ThenInclude(m => m.Patient)
                .Include(i => i.Routine)
                .Where(i => i.Medication.PatientId == patientId)
                .ToListAsync();
        }
        
        public async Task<IntakeInstruction> CreateAsync(CreateIntakeInstructionRequestDto request)
        {
            var instruction = new IntakeInstruction
            {
                MedicationId = request.MedicationId,
                DosePerIntake = request.DosePerIntake,
                DosageRegime = request.DosageRegime,
                Time = request.Time,
                RoutineId = request.RoutineId
            };

            _context.IntakeInstructions.Add(instruction);
            await _context.SaveChangesAsync();
            return instruction;
        }
        
        public async Task<IntakeInstruction?> UpdateAsync(int id, UpdateIntakeInstructionRequestDto request)
        {
            var instruction = await _context.IntakeInstructions.FindAsync(id);
            if (instruction == null)
                return null;

            instruction.DosePerIntake = request.DosePerIntake;
            instruction.DosageRegime = request.DosageRegime;
            instruction.Time = request.Time;
            instruction.RoutineId = request.RoutineId;

            await _context.SaveChangesAsync();
            return instruction;
        }
        
        public async Task<bool> DeleteAsync(int id)
        {
            var instruction = await _context.IntakeInstructions.FindAsync(id);
            if (instruction == null)
                return false;

            _context.IntakeInstructions.Remove(instruction);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}