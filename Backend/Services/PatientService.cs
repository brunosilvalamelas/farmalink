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
    public async Task<Patient> CreatePatientAsync(PatientRequestDto patientDto)
    {
        var errors = new List<ValidationError>();

        if (await _context.Patients.AnyAsync(p => p.Email == patientDto.Email))
            errors.Add(new ValidationError { Field = "email", Message = "Já existe um utente com esse email." });

        if (await _context.Patients.AnyAsync(p => p.PhoneNumber == patientDto.PhoneNumber))
            errors.Add(new ValidationError
                { Field = "phoneNumber", Message = "Já existe um utente com esse número de telemóvel." });

        if (errors.Count > 0)
            throw new ValidationException(errors);

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
        return newPatient;
    }

    /// <summary>
    /// Retrieves all patients from the database.
    /// </summary>
    /// <returns>A ServiceResult containing the list of patients.</returns>
    public async Task<List<Patient>> GetAllPatientsAsync()
    {
        var patients = await _context.Patients.ToListAsync();
        return patients;
    }

    /// <summary>
    /// Retrieves a patient by their ID.
    /// </summary>
    /// <param name="id">The ID of the patient to retrieve.</param>
    /// <returns>A ServiceResult containing the patient or a not found result.</returns>
    public async Task<Patient?> GetPatientByIdAsync(int id)
    {
        var patient = await _context.Patients.FindAsync(id);

        return patient;
    }

    /// <summary>
    /// Updates an existing patient with new data.
    /// </summary>
    /// <param name="id">The ID of the patient to update.</param>
    /// <param name="patientDto">The updated patient data.</param>
    /// <returns>A ServiceResult indicating the success of the update operation.</returns>
    public async Task<bool> UpdatePatientAsync(int id, PatientRequestDto patientDto)
    {
        var patient = await _context.Patients.FindAsync(id);

        if (patient == null)
        {
            return false;
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
            throw new ValidationException(errors);
        }

        _context.Entry(patient).CurrentValues.SetValues(patientDto);

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Deletes a patient by their ID.
    /// </summary>
    /// <param name="id">The ID of the patient to delete.</param>
    /// <returns>A ServiceResult indicating the success of the delete operation.</returns>
    public async Task<bool> DeletePatientAsync(int id)
    {
        var patient = await _context.Patients.FindAsync(id);
        if (patient == null)
        {
            return false;
        }

        _context.Patients.Remove(patient);
        await _context.SaveChangesAsync();
        return true;
    }
}