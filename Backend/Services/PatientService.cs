using Backend.Data;
using Backend.Entities;
using Backend.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

/// <summary>
/// Service class for handling patient-related business logic operations.
/// </summary>
public class PatientService
{
    private readonly DataContext _context;

    /// <summary>
    /// Initializes a new instance of the PatientService.
    /// </summary>
    /// <param name="dataContext">The data context instance for database interactions.</param>
    public PatientService(DataContext dataContext)
    {
        _context = dataContext;
    }

    /// <summary>
    /// Creates a new patient in the database.
    /// </summary>
    /// <param name="newPatient">The patient entity to create.</param>
    /// <returns>A ServiceResult containing the created patient or validation errors.</returns>
    public async Task<ServiceResult<Patient>> CreatePatient(Patient newPatient)
    {
        var errors = new List<ValidationError>();

        if (await _context.Patients.AnyAsync(p => p.Email == newPatient.Email))
            errors.Add(new ValidationError { Field = "email", Message = "Já existe um paciente com esse email." });

        if (await _context.Patients.AnyAsync(p => p.PhoneNumber == newPatient.PhoneNumber))
            errors.Add(new ValidationError
                { Field = "phoneNumber", Message = "Já existe um paciente com esse número de telemóvel." });

        if (errors.Count > 0)
            return ServiceResult<Patient>.ValidationError(errors);

        await _context.Patients.AddAsync(newPatient);
        await _context.SaveChangesAsync();
        return ServiceResult<Patient>.Ok(newPatient, "Paciente criado");
    }

    /// <summary>
    /// Retrieves all patients from the database.
    /// </summary>
    /// <returns>A ServiceResult containing the list of patients.</returns>
    public async Task<ServiceResult<List<Patient>>> GetAllPatients()
    {
        var patients = await _context.Patients.ToListAsync();
        return ServiceResult<List<Patient>>.Ok(patients, "Pacientes encontrados.");
    }

    /// <summary>
    /// Retrieves a patient by their ID.
    /// </summary>
    /// <param name="id">The ID of the patient to retrieve.</param>
    /// <returns>A ServiceResult containing the patient or a not found result.</returns>
    public async Task<ServiceResult<Patient>> GetPatientById(int id)
    {
        var existingPatient = await _context.Patients.FindAsync(id);
        if (existingPatient == null)
        {
            return ServiceResult<Patient>.NotFound("Não existe nenhum paciente com esse id.");
        }

        return ServiceResult<Patient>.Ok(existingPatient, "Paciente encontrado.");
    }

    /// <summary>
    /// Updates an existing patient with new data.
    /// </summary>
    /// <param name="id">The ID of the patient to update.</param>
    /// <param name="updatedPatient">The updated patient data.</param>
    /// <returns>A ServiceResult indicating the success of the update operation.</returns>
    public async Task<ServiceResult<Patient>> UpdatePatient(int id, Patient updatedPatient)
    {
        var existing = await _context.Patients.FindAsync(id);

        if (existing == null)
        {
            return ServiceResult<Patient>.NotFound("Não existe nenhum paciente com esse id.");
        }

        var errors = new List<ValidationError>();

        if (await _context.Patients
                .AnyAsync(p => p.Email == updatedPatient.Email && p.Id != id))
            errors.Add(new ValidationError { Field = "email", Message = "Já existe um paciente com esse email." });

        if (await _context.Patients
                .AnyAsync(p => p.PhoneNumber == updatedPatient.PhoneNumber && p.Id != id))
            errors.Add(new ValidationError
                { Field = "phoneNumber", Message = "O número de telefone já está em uso por outro paciente." });

        if (errors.Count > 0)
        {
            return ServiceResult<Patient>.ValidationError(errors);
        }


        existing.Name = updatedPatient.Name;
        existing.Email = updatedPatient.Email;
        existing.PhoneNumber = updatedPatient.PhoneNumber;
        existing.Address = updatedPatient.Address;
        existing.ZipCode = updatedPatient.ZipCode;

        await _context.SaveChangesAsync();
        return ServiceResult<Patient>.Ok(null, "Dados do paciente atualizados.");
    }

    /// <summary>
    /// Deletes a patient by their ID.
    /// </summary>
    /// <param name="id">The ID of the patient to delete.</param>
    /// <returns>A ServiceResult indicating the success of the delete operation.</returns>
    public async Task<ServiceResult<Patient>> DeletePatient(int id)
    {
        var patient = await _context.Patients.FindAsync(id);
        if (patient == null)
        {
            return ServiceResult<Patient>.NotFound("Não existe nenhum paciente com esse id.");
        }

        _context.Patients.Remove(patient);
        await _context.SaveChangesAsync();
        return ServiceResult<Patient>.Ok(null, "Paciente removido.");
    }
}