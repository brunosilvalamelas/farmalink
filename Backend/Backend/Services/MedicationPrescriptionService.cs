using Backend.Data;
using Backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
    public class MedicationPrescriptionService : IMedicationPrescriptionService

    {
        private readonly DataContext _context;

        public MedicationPrescriptionService(DataContext context)
        {
            _context = context;
        }

       
        public async Task<List<Medication>> GetAllAsync(bool? prescribed = null)
        {
            var query = _context.Medications.AsQueryable();

            if (prescribed.HasValue)
                query = query.Where(m => m.RequiresPrescription == prescribed.Value);

            return await query.ToListAsync();
        }

       
        public async Task<List<Medication>> GetByPatientIdAsync(int patientId, bool? prescribed = null)
        {
            var query = _context.Medications.Where(m => m.PatientId == patientId);

            if (prescribed.HasValue)
                query = query.Where(m => m.RequiresPrescription == prescribed.Value);

            return await query.ToListAsync();
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
            existing.RequiresPrescription = updatedMedication.RequiresPrescription;
            existing.PatientId = updatedMedication.PatientId;

            await _context.SaveChangesAsync();
            return existing;
        }

        
        public async Task<Medication> AddAsync(Medication medication)
        {
            _context.Medications.Add(medication);
            await _context.SaveChangesAsync();
            return medication;
        }

        
        public async Task<Medication> AddToPatientAsync(int patientId, Medication medication)
        {
            medication.PatientId = patientId;
            _context.Medications.Add(medication);
            await _context.SaveChangesAsync();
            return medication;
        }

        
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