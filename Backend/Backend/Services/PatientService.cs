using Backend.Data;
using Backend.DTOs.request;
using Backend.Entities;
using Backend.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

/// <summary>
/// Service class for handling patient-related business logic operations.
/// </summary>
public class PatientService : IPatientService
{
    private readonly DataContext _context;
    private readonly IUserService _userService;

    /// <summary>
    /// Initializes a new instance of the PatientService.
    /// </summary>
    /// <param name="dataContext">The data context instance for database interactions.</param>
    public PatientService(DataContext dataContext, IUserService userService)
    {
        _context = dataContext;
        _userService = userService;
    }

    /// <summary>
    /// Creates a new patient linked to the specified tutor, validating for duplicates.
    /// </summary>
    /// <param name="loggedInTutorId">The ID of the tutor creating the patient.</param>
    /// <param name="createPatientDto">The patient creation data.</param>
    /// <returns>The created Patient entity or null if tutor doesn't exist.</returns>
    public async Task<Patient?> CreatePatientAsync(int loggedInTutorId, CreatePatientRequestDto createPatientDto)
    {
        var tutorExists = await _context.Tutors.AnyAsync(t => t.Id == loggedInTutorId);
        if (!tutorExists)
        {
            return null;
        }

        var fields = new Dictionary<string, string>
        {
            { "Email", createPatientDto.Email },
            { "PhoneNumber", createPatientDto.PhoneNumber }
        };

        await _userService.ValidateDuplicatesAsync<User>(fields);

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(createPatientDto.Password);

        Patient newPatient = new Patient
        {
            Name = createPatientDto.Name,
            Email = createPatientDto.Email,
            PasswordHash = hashedPassword,
            PhoneNumber = createPatientDto.PhoneNumber,
            ZipCode = createPatientDto.ZipCode,
            Address = createPatientDto.Address,
            TutorId = loggedInTutorId,
            Role = UserRole.Patient,
        };

        _context.Patients.Add(newPatient);
        await _context.SaveChangesAsync();

        return newPatient;
    }

    /// <summary>
    /// Retrieves all patients from the database.
    /// </summary>
    /// <returns>The list of patients.</returns>
    public async Task<List<Patient>> GetAllPatientsAsync()
    {
        var patients = await _context.Patients.Include(p => p.Tutor).ToListAsync();
        return patients;
    }

    /// <summary>
    /// Retrieves a patient by their ID.
    /// </summary>
    /// <param name="id">The ID of the patient to retrieve.</param>
    /// <returns>The patient if found, or null otherwise.</returns>
    public async Task<Patient?> GetPatientByIdAsync(int id)
    {
        var patient = await _context.Patients.Include(p => p.Tutor).FirstOrDefaultAsync(p => p.Id == id);

        return patient;
    }

    /// <summary>
    /// Retrieves patients by their tutor's ID.
    /// </summary>
    /// <param name="tutorId">The ID of the tutor to retrieve patients for.</param>
    /// <returns>A list of patients or null if the tutor does not exist.</returns>
    public async Task<List<Patient>?> GetPatientsByTutorIdAsync(int tutorId)
    {
        var tutorExists = await _context.Tutors.AnyAsync(t => t.Id == tutorId);
        if (!tutorExists)
        {
            return null;
        }

        var patients = await _context.Patients.Where(p => p.TutorId == tutorId).ToListAsync();

        return patients;
    }

    /// <summary>
    /// Updates an existing patient with new data.
    /// </summary>
    /// <param name="id">The ID of the patient to update.</param>
    /// <param name="updatePatientDto">The updated patient data.</param>
    /// <returns>True if the update was successful, false otherwise.</returns>
    public async Task<bool> UpdatePatientAsync(int id, UpdatePatientRequestDto updatePatientDto)
    {
        var patient = await _context.Patients.FindAsync(id);

        if (patient == null)
        {
            return false;
        }

        patient.Name = updatePatientDto.Name;
        patient.ZipCode = updatePatientDto.ZipCode;
        patient.Address = updatePatientDto.Address;

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Deletes a patient by their ID.
    /// </summary>
    /// <param name="id">The ID of the patient to delete.</param>
    /// <returns>True if the deletion was successful, false otherwise.</returns>
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