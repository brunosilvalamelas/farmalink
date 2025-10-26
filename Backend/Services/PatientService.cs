using Backend.Data;
using Backend.DTOs.request;
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
    /// <param name="patientDto">The data to create a new patient.</param>
    /// <returns>A ServiceResult containing the created patient or validation errors.</returns>
    public async Task<ServiceResult<Patient>> CreatePatientAsync(PatientRequestDto patientDto)
    {
        var errors = new List<ValidationError>();

        if (await _context.Patients.AnyAsync(p => p.Email == patientDto.Email))
            errors.Add(new ValidationError { Field = "email", Message = "Já existe um utente com esse email." });

        if (await _context.Patients.AnyAsync(p => p.PhoneNumber == patientDto.PhoneNumber))
            errors.Add(new ValidationError
                { Field = "phoneNumber", Message = "Já existe um utente com esse número de telemóvel." });

        if (errors.Count > 0)
            return ServiceResult<Patient>.ValidationError(errors);

        var newPatient = new Patient
        {
            Name = patientDto.Name,
            Email = patientDto.Email,
            PhoneNumber = patientDto.PhoneNumber,
            Address = patientDto.Address,
            ZipCode = patientDto.ZipCode
        };

        await _context.Patients.AddAsync(newPatient);
        await _context.SaveChangesAsync();
        return ServiceResult<Patient>.Ok(newPatient, "Utente criado");
    }

    /// <summary>
    /// Retrieves all patients from the database.
    /// </summary>
    /// <returns>A ServiceResult containing the list of patients.</returns>
    public async Task<ServiceResult<List<Patient>>> GetAllPatientsAsync()
    {
        var patients = await _context.Patients.ToListAsync();
        return ServiceResult<List<Patient>>.Ok(patients, "Utentes encontrados.");
    }

    /// <summary>
    /// Retrieves a patient by their ID.
    /// </summary>
    /// <param name="id">The ID of the patient to retrieve.</param>
    /// <returns>A ServiceResult containing the patient or a not found result.</returns>
    public async Task<ServiceResult<Patient>> GetPatientByIdAsync(int id)
    {
        var existingPatient = await _context.Patients.FindAsync(id);
        if (existingPatient == null)
        {
            return ServiceResult<Patient>.NotFound("Não existe nenhum utente com esse id.");
        }

        return ServiceResult<Patient>.Ok(existingPatient, "Utente encontrado.");
    }

    /// <summary>
    /// Updates an existing patient with new data.
    /// </summary>
    /// <param name="id">The ID of the patient to update.</param>
    /// <param name="patientDto">The updated patient data.</param>
    /// <returns>A ServiceResult indicating the success of the update operation.</returns>
    public async Task<ServiceResult<Patient>> UpdatePatientAsync(int id, PatientRequestDto patientDto)
    {
        var existing = await _context.Patients.FindAsync(id);

        if (existing == null)
        {
            return ServiceResult<Patient>.NotFound("Não existe nenhum utente com esse id.");
        }

        var errors = new List<ValidationError>();

        if (await _context.Patients
                .AnyAsync(p => p.Email == patientDto.Email && p.Id != id))
            errors.Add(new ValidationError { Field = "email", Message = "Já existe um utente com esse email." });

        if (await _context.Patients
                .AnyAsync(p => p.PhoneNumber == patientDto.PhoneNumber && p.Id != id))
            errors.Add(new ValidationError
                { Field = "phoneNumber", Message = "O número de telefone já está em uso por outro utente." });

        if (errors.Count > 0)
        {
            return ServiceResult<Patient>.ValidationError(errors);
        }

        existing.Name = patientDto.Name;
        existing.Email = patientDto.Email;
        existing.PhoneNumber = patientDto.PhoneNumber;
        existing.Address = patientDto.Address;
        existing.ZipCode = patientDto.ZipCode;

        await _context.SaveChangesAsync();
        return ServiceResult<Patient>.Ok(null, "Dados do utente atualizados.");
    }

    /// <summary>
    /// Deletes a patient by their ID.
    /// </summary>
    /// <param name="id">The ID of the patient to delete.</param>
    /// <returns>A ServiceResult indicating the success of the delete operation.</returns>
    public async Task<ServiceResult<Patient>> DeletePatientAsync(int id)
    {
        var patient = await _context.Patients.FindAsync(id);
        if (patient == null)
        {
            return ServiceResult<Patient>.NotFound("Não existe nenhum utente com esse id.");
        }

        _context.Patients.Remove(patient);
        await _context.SaveChangesAsync();
        return ServiceResult<Patient>.Ok(null, "Utente removido.");
    }
}