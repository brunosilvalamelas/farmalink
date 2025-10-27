using Backend.Data;
using Backend.DTOs.request;
using Backend.Entities;
using Backend.Services;
using Microsoft.EntityFrameworkCore;

namespace BackendTests.Integration.Services;

public class PatientServiceTests : IDisposable
{
    private readonly DataContext _dataContext;
    private readonly PatientService _patientService;

    private readonly int TutorId;
    private readonly int Patient1Id;
    private readonly int Patient2Id;

    public PatientServiceTests()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dataContext = new DataContext(options);

        // Seed Tutor
        var tutor = new Tutor
        {
            Name = "João Silva",
            Email = "joao@example.com",
            PhoneNumber = "912345678",
            Address = "Rua do João",
            ZipCode = "1234-567"
        };
        _dataContext.Tutors.Add(tutor);
        _dataContext.SaveChanges();

        TutorId = tutor.Id;

        // Seed Patients
        var patient1 = new Patient
        {
            Name = "Maria Santos",
            Email = "maria@example.com",
            PhoneNumber = "912345679",
            Address = "Rua da Maria",
            ZipCode = "1234-568",
            TutorId = TutorId
        };

        var patient2 = new Patient
        {
            Name = "Pedro Costa",
            Email = "pedro@example.com",
            PhoneNumber = "912345680",
            Address = "Rua do Pedro",
            ZipCode = "1234-569",
            TutorId = TutorId
        };

        _dataContext.Patients.AddRange(patient1, patient2);
        _dataContext.SaveChanges();

        Patient1Id = patient1.Id;
        Patient2Id = patient2.Id;

        _patientService = new PatientService(_dataContext);
    }

    public void Dispose()
    {
        _dataContext.Database.EnsureDeleted();
        _dataContext.Dispose();
    }

    [Fact]
    public async Task CreatePatientAsyncShouldNotBeNull()
    {
        var newPatient = new PatientRequestDto
        {
            Name = "Ana Pereira",
            Email = "ana@example.com",
            PhoneNumber = "912345681",
            Address = "Rua da Ana",
            ZipCode = "1234-570"
        };

        var result = await _patientService.CreatePatientAsync(TutorId, newPatient);

        Assert.NotNull(result);
        Assert.Equal(newPatient.Name, result.Name);
    }

    [Fact]
    public async Task CreatePatientAsyncShouldBeNullWhenTutorNotExists()
    {
        var newPatient = new PatientRequestDto
        {
            Name = "Ana Pereira",
            Email = "ana@example.com",
            PhoneNumber = "912345681",
            Address = "Rua da Ana",
            ZipCode = "1234-570"
        };

        var result = await _patientService.CreatePatientAsync(999, newPatient);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllPatientsAsyncShouldNotBeEmpty()
    {
        var patients = await _patientService.GetAllPatientsAsync();

        Assert.NotEmpty(patients);
        Assert.Equal(2, patients.Count);
    }

    [Fact]
    public async Task GetPatientByIdAsyncShouldNotBeNull()
    {
        var result = await _patientService.GetPatientByIdAsync(Patient1Id);

        Assert.NotNull(result);
        Assert.Equal(Patient1Id, result.Id);
    }

    [Fact]
    public async Task GetPatientByIdAsyncShouldBeNull()
    {
        var result = await _patientService.GetPatientByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetPatientsByTutorIdAsyncShouldNotBeNull()
    {
        var result = await _patientService.GetPatientsByTutorIdAsync(TutorId);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetPatientsByTutorIdAsyncShouldBeNull()
    {
        var result = await _patientService.GetPatientsByTutorIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdatePatientAsyncShouldReturnTrue()
    {
        var updatedPatient = new PatientRequestDto
        {
            Name = "Updated Maria",
            Email = "updated@example.com",
            PhoneNumber = "965432109",
            Address = "Updated Address",
            ZipCode = "9876-543"
        };

        var result = await _patientService.UpdatePatientAsync(Patient1Id, updatedPatient);

        Assert.True(result);
    }

    [Fact]
    public async Task UpdatePatientAsyncShouldReturnFalse()
    {
        var updatedPatient = new PatientRequestDto
        {
            Name = "Updated Maria",
            Email = "updated@example.com",
            PhoneNumber = "965432109",
            Address = "Updated Address",
            ZipCode = "9876-543"
        };

        var result = await _patientService.UpdatePatientAsync(999, updatedPatient);

        Assert.False(result);
    }

    [Fact]
    public async Task DeletePatientAsyncShouldReturnTrue()
    {
        var result = await _patientService.DeletePatientAsync(Patient1Id);

        Assert.True(result);
    }

    [Fact]
    public async Task DeletePatientAsyncShouldReturnFalse()
    {
        var result = await _patientService.DeletePatientAsync(999);

        Assert.False(result);
    }
}
