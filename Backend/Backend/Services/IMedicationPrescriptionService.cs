using Backend.Entities;

namespace Backend.Services
{
    public interface IMedicationPrescriptionService
    {
        Task<List<Medication>> GetAllAsync(bool? prescribed = null);
        Task<List<Medication>> GetByPatientIdAsync(int patientId, bool? prescribed = null);
        Task<Medication?> GetByIdAsync(int id);
        Task<Medication?> UpdateAsync(int id, Medication updatedMedication);
        Task<Medication> AddAsync(Medication medication);
        Task<Medication> AddToPatientAsync(int patientId, Medication medication);
        Task<bool> DeleteAsync(int id);
    }
}