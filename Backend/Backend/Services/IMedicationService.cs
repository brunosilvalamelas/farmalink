// Services/IMedicationService.cs
using Backend.Entities;

namespace Backend.Services
{
    public interface IMedicationService
    {
        Task<List<Medication>> GetAllAsync();
        Task<Medication?> GetByIdAsync(int id);
        Task<List<Medication>> GetMedicationsByPatientIdAsync(int patientId);
        Task<Medication?> UpdateAsync(int id, Medication updatedMedication);
        Task<Medication> AddAsync(Medication medication);
        Task<bool> DeleteAsync(int id);
    }
}