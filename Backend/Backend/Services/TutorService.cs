using Backend.Data;
using Backend.DTOs.request;
using Backend.Entities;
using Backend.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

/// <summary>
/// Service class for handling tutor-related business logic operations.
/// </summary>
public class TutorService
{
    private readonly DataContext _context;

    /// <summary>
    /// Initializes a new instance of the TutorService.
    /// </summary>
    /// <param name="dataContext">The data context instance for database interactions.</param>
    public TutorService(DataContext dataContext)
    {
        _context = dataContext;
    }

    /// <summary>
    /// Creates a new tutor in the database.
    /// </summary>
    /// <param name="tutorDto">The data to create a new tutor.</param>
    /// <returns>A ServiceResult containing the created tutor or validation errors.</returns>
    public async Task<Tutor> CreateTutorAsync(TutorRequestDto tutorDto)
    {
        var errors = new List<ValidationError>();

        var tutors = await _context.Tutors
            .Select(p => new { p.Email, p.PhoneNumber })
            .ToListAsync();

        if (tutors.Any(p => p.Email == tutorDto.Email))
            errors.Add(new ValidationError { Field = "email", Message = "Já existe um tutor com esse email." });

        if (tutors.Any(p => p.PhoneNumber == tutorDto.PhoneNumber))
            errors.Add(new ValidationError
                { Field = "phoneNumber", Message = "Já existe um tutor com esse número de telemóvel." });

        if (errors.Count > 0)
            throw new ValidationException(errors);

        var newTutor = new Tutor
        {
            Name = tutorDto.Name,
            Email = tutorDto.Email,
            PhoneNumber = tutorDto.PhoneNumber,
            Address = tutorDto.Address,
            ZipCode = tutorDto.ZipCode
        };

        await _context.Tutors.AddAsync(newTutor);
        await _context.SaveChangesAsync();
        return newTutor;
    }

    /// <summary>
    /// Retrieves all tutors from the database.
    /// </summary>
    /// <returns>A ServiceResult containing the list of tutors.</returns>
    public async Task<List<Tutor>> GetAllTutorsAsync()
    {
        var tutors = await _context.Tutors.ToListAsync();
        return tutors;
    }

    /// <summary>
    /// Retrieves a tutor by their ID.
    /// </summary>
    /// <param name="id">The ID of the tutor to retrieve.</param>
    /// <returns>A ServiceResult containing the tutor or a not found result.</returns>
    public async Task<Tutor?> GetTutorByIdAsync(int id)
    {
        var tutor = await _context.Tutors.FindAsync(id);

        return tutor;
    }

    /// <summary>
    /// Updates an existing tutor with new data.
    /// </summary>
    /// <param name="id">The ID of the tutor to update.</param>
    /// <param name="tutorDto">The updated tutor data.</param>
    /// <returns>A ServiceResult indicating the success of the update operation.</returns>
    public async Task<bool> UpdateTutorAsync(int id, TutorRequestDto tutorDto)
    {
        var tutor = await _context.Tutors.FindAsync(id);

        if (tutor == null)
        {
            return false;
        }

        var errors = new List<ValidationError>();

        if (await _context.Tutors
                .AnyAsync(p => p.Email == tutorDto.Email && p.Id != id))
            errors.Add(new ValidationError { Field = "email", Message = "Já existe um tutor com esse email." });

        if (await _context.Tutors
                .AnyAsync(p => p.PhoneNumber == tutorDto.PhoneNumber && p.Id != id))
            errors.Add(new ValidationError
                { Field = "phoneNumber", Message = "O número de telefone já está em uso por outro tutor." });

        if (errors.Count > 0)
        {
            throw new ValidationException(errors);
        }

        _context.Entry(tutor).CurrentValues.SetValues(tutorDto);

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Deletes a tutor by their ID.
    /// </summary>
    /// <param name="id">The ID of the tutor to delete.</param>
    /// <returns>A ServiceResult indicating the success of the delete operation.</returns>
    public async Task<bool> DeleteTutorAsync(int id)
    {
        var tutor = await _context.Tutors.FindAsync(id);
        if (tutor == null)
        {
            return false;
        }

        _context.Tutors.Remove(tutor);
        await _context.SaveChangesAsync();
        return true;
    }
}

