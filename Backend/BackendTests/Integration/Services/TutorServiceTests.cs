using Backend.Data;
using Backend.DTOs.request;
using Backend.Entities;
using Backend.Services;
using Microsoft.EntityFrameworkCore;

namespace BackendTests.Integration.Services;

public class TutorServiceTests : IDisposable
{
    private readonly DataContext _dataContext;
    private readonly TutorService _tutorService;

    // Store generated IDs of seeded tutors
    private readonly int Tutor1Id;
    private readonly int Tutor2Id;

    /// <summary>
    /// Assigns the context to the TutorServiceTests class
    /// </summary>
    public TutorServiceTests()
    {
        // Unique in-memory database per test run
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dataContext = new DataContext(options);

        // Seed data without setting Ids manually
        var tutor1 = new Tutor
        {
            Name = "João Silva",
            Email = "joao@example.com",
            PhoneNumber = "912345678",
            Address = "Rua do João",
            ZipCode = "1234-567"
        };

        var tutor2 = new Tutor
        {
            Name = "Pedro Costa",
            Email = "pedro@example.com",
            PhoneNumber = "912345679",
            Address = "Rua do Pedro",
            ZipCode = "1234-568"
        };

        _dataContext.Tutors.AddRange(tutor1, tutor2);
        _dataContext.SaveChanges();

        // Capture the auto-generated IDs
        Tutor1Id = tutor1.Id;
        Tutor2Id = tutor2.Id;

        _tutorService = new TutorService(_dataContext);
    }

    /// <summary>
    /// Dispose method to delete the database after the tests are run
    /// </summary>
    public void Dispose()
    {
        _dataContext.Database.EnsureDeleted();
        _dataContext.Dispose();
    }

    [Fact]
    public async Task CreateTutorAsyncShouldNotBeNull()
    {
        var newTutor = new TutorRequestDto
        {
            Name = "Ana Pereira",
            Email = "ana@example.com",
            PhoneNumber = "912345681",
            Address = "Rua da Ana",
            ZipCode = "1234-570"
        };

        var result = await _tutorService.CreateTutorAsync(newTutor);

        Assert.NotNull(result);
        Assert.Equal(newTutor.Name, result.Name);
    }

    [Fact]
    public async Task GetAllTutorsAsyncShouldNotBeEmpty()
    {
        var tutors = await _tutorService.GetAllTutorsAsync();

        Assert.NotEmpty(tutors);
        Assert.Equal(2, tutors.Count); // Seeded tutors
    }

    [Fact]
    public async Task GetTutorByIdAsyncShouldNotBeNull()
    {
        var result = await _tutorService.GetTutorByIdAsync(Tutor1Id);

        Assert.NotNull(result);
        Assert.Equal(Tutor1Id, result.Id);
    }

    [Fact]
    public async Task GetTutorByIdAsyncShouldBeNull()
    {
        var result = await _tutorService.GetTutorByIdAsync(999); // Non-existent ID
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateTutorAsyncShouldReturnTrue()
    {
        var updatedTutor = new TutorRequestDto
        {
            Name = "Updated João",
            Email = "updated@example.com",
            PhoneNumber = "965432109",
            Address = "Updated Address",
            ZipCode = "9876-543"
        };

        var result = await _tutorService.UpdateTutorAsync(Tutor1Id, updatedTutor);

        Assert.True(result);
    }

    [Fact]
    public async Task UpdateTutorAsyncShouldReturnFalse()
    {
        var updatedTutor = new TutorRequestDto
        {
            Name = "Updated João",
            Email = "updated@example.com",
            PhoneNumber = "965432109",
            Address = "Updated Address",
            ZipCode = "9876-543"
        };

        var result = await _tutorService.UpdateTutorAsync(999, updatedTutor); // Non-existent ID
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteTutorAsyncShouldReturnTrue()
    {
        var result = await _tutorService.DeleteTutorAsync(Tutor1Id);
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteTutorAsyncShouldReturnFalse()
    {
        var result = await _tutorService.DeleteTutorAsync(999); // Non-existent ID
        Assert.False(result);
    }
}
