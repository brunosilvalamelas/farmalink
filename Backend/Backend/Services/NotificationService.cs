namespace Backend.Services;

using Backend.Data;
using Backend.Entities;
using Microsoft.EntityFrameworkCore;


public class NotificationService
{
    private readonly DataContext _context;
    
    
    public NotificationService(DataContext context)
    {
        _context = context;
    }

    public async Task CreateMedicationReminderAsync(IntakeInstruction instruction)
    {
        var patientNotification = new Notification
        {
            RecipientId = instruction.Medication.PatientId,
            RecipientType = "Patient",
            Message = $"Lembrete: Tomar {instruction.DosePerIntake} de {instruction.Medication.Name} às {instruction.Time:hh\\:mm}",
            Type = "MedicationReminder",
            DateTime = DateTime.UtcNow
        };
        
        var tutorNotification = new Notification
        {
            RecipientId = 1, // Temporário - depois usaremos o tutor 
            RecipientType = "Tutor", 
            Message = $"Lembrete: O paciente deve tomar {instruction.DosePerIntake} de {instruction.Medication.Name} às {instruction.Time:hh\\:mm}",
            Type = "MedicationReminder",
            DateTime = DateTime.UtcNow
        };
        
        _context.Notifications.Add(patientNotification);
        _context.Notifications.Add(tutorNotification);
            
        await _context.SaveChangesAsync();
        
    }
}