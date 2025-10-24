using Backend.Data;
using Backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class PatientService
{
    private readonly DataContext _context;


    public PatientService(DataContext dataContext)
    {
        _context = dataContext;
    }


    public async Task<Patient> CreatePatient(Patient newPatient)
    {
        var existingPatients = await _context.Patients
            .Where(p => p.Email == newPatient.Email || p.PhoneNumber == newPatient.PhoneNumber)
            .ToListAsync();

        var errors = new List<string>();

        if (existingPatients.Any(p => p.Email == newPatient.Email))
            errors.Add("Já existe um paciente com esse email.");

        if (existingPatients.Any(p => p.PhoneNumber == newPatient.PhoneNumber))
            errors.Add("Já existe um paciente com esse número de telemóvel.");

        if (errors.Any())
            throw new InvalidOperationException(string.Join(" | ", errors));

        await _context.Patients.AddAsync(newPatient);
        await _context.SaveChangesAsync();
        return newPatient;
    }

    public async Task<List<Patient>> GetAllPatients()
    {
        return await _context.Patients.ToListAsync();
    }

    public async Task<Patient?> GetPatientById(int id)
    {
        return await _context.Patients.FindAsync(id);
    }

    public async Task UpdatePatient(int id, Patient updatedPatient)
    {
        var existing = await _context.Patients.FindAsync(id);
        if (existing == null)
            throw new KeyNotFoundException("Paciente não encontrado.");

        bool emailExists = await _context.Patients
            .AnyAsync(p => p.Email == updatedPatient.Email && p.Id != id);

        if (emailExists)
            throw new InvalidOperationException("O email já está em uso por outro paciente.");

        bool phoneExists = await _context.Patients
            .AnyAsync(p => p.PhoneNumber == updatedPatient.PhoneNumber && p.Id != id);

        if (phoneExists)
            throw new InvalidOperationException("O número de telefone já está em uso por outro paciente.");

        existing.Name = updatedPatient.Name;
        existing.Email = updatedPatient.Email;
        existing.PhoneNumber = updatedPatient.PhoneNumber;
        existing.Address = updatedPatient.Address;
        existing.ZipCode = updatedPatient.ZipCode;

        await _context.SaveChangesAsync();
    }

    public async Task DeletePatient(int id)
    {
        var patient = await _context.Patients.FindAsync(id);
        if (patient == null)
            throw new KeyNotFoundException("Paciente não encontrado.");

        _context.Patients.Remove(patient);
        await _context.SaveChangesAsync();
    }
}