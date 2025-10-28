using Backend.Data;
using Backend.DTOs.request;
using Backend.Entities;
using Backend.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

/// <summary>
/// Service class for handling tutor-related business logic operations.
/// </summary>
public class TutorService : ITutorService
{
    private readonly DataContext _context;
    private readonly IUserService _userService;

    /// <summary>
    /// Initializes a new instance of the TutorService.
    /// </summary>
    /// <param name="dataContext">The data context instance for database interactions.</param>
    public TutorService(DataContext dataContext, IUserService userService)
    {
        _context = dataContext;
        _userService = userService;
    }

    /// <summary>
    /// Creates a new tutor, validates for duplicates, and generates a JWT token.
    /// </summary>
    /// <param name="createTutorDto">The tutor creation data.</param>
    /// <returns>A tuple containing the created Tutor entity and the JWT token.</returns>
    public async Task<(Tutor tutor, string token)> CreateTutorAsync(CreateTutorRequestDto createTutorDto)
    {
        var fields = new Dictionary<string, string>
        {
            { "Email", createTutorDto.Email },
            { "PhoneNumber", createTutorDto.PhoneNumber }
        };

        await _userService.ValidateDuplicatesAsync<User>(fields);

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(createTutorDto.Password);

        Tutor newTutor = new Tutor
        {
            Name = createTutorDto.Name,
            Email = createTutorDto.Email,
            PasswordHash = hashedPassword,
            PhoneNumber = createTutorDto.PhoneNumber,
            ZipCode = createTutorDto.ZipCode,
            Address = createTutorDto.Address,
            Role = UserRole.Tutor
        };

        _context.Tutors.Add(newTutor);
        await _context.SaveChangesAsync();

        var token = _userService.GenerateToken(newTutor.Id, newTutor.Role);

        return (newTutor, token);
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
    /// <param name="updateTutorDto">The updated tutor data.</param>
    /// <returns>A ServiceResult indicating the success of the update operation.</returns>
    public async Task<bool> UpdateTutorAsync(int id, UpdateTutorRequestDto updateTutorDto)
    {
        var tutor = await _context.Tutors.FindAsync(id);

        if (tutor == null)
        {
            return false;
        }

        tutor.Name = updateTutorDto.Name;
        tutor.Address = updateTutorDto.Address;
        tutor.ZipCode = updateTutorDto.ZipCode;

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
