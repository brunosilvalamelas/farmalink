using Backend.Data;
using Backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
    public class MedicationService
    {
        private readonly DataContext _context;

        public MedicationService(DataContext context)
        {
            _context = context;
        }

        public async Task<List<Medication>> GetAllAsync()
        {
            return await _context.Medications.ToListAsync();
        }

        public async Task<Medication?> GetByIdAsync(int id)
        {
            return await _context.Medications
                .FirstOrDefaultAsync(m => m.MedicationId == id);
        }

        public async Task<Medication?> UpdateAsync(int id, Medication updatedMedication)
        {
            var existing = await _context.Medications.FindAsync(id);
            if (existing == null)
                return null;

            existing.Name = updatedMedication.Name;
            existing.QuantityOnHand = updatedMedication.QuantityOnHand;
            existing.QuantityPerUnit = updatedMedication.QuantityPerUnit;
            existing.LowStockThreshold = updatedMedication.LowStockThreshold;
            existing.PatientId = updatedMedication.PatientId;

            await _context.SaveChangesAsync();
            return existing;
        }

        // ✅ Adicionar medicamento (POST)
        public async Task<Medication> AddAsync(Medication medication)
        {
            _context.Medications.Add(medication);
            await _context.SaveChangesAsync();
            return medication;
        }

        // ✅ Remover medicamento (DELETE)
        public async Task<bool> DeleteAsync(int id)
        {
            var medication = await _context.Medications.FindAsync(id);
            if (medication == null)
                return false;

            _context.Medications.Remove(medication);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}