using Backend.Controllers;
using Backend.Data;
using Backend.Entities;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendTests.Integration.Controllers;

public class PatientMedicationsControllerTests : IDisposable
{
    private readonly DataContext _context;
    private readonly MedicationPrescriptionService _service;
    private readonly PatientMedicationsController _controller;

    public PatientMedicationsControllerTests()
    {
       
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new DataContext(options);

        
        var patient = new Patient
        {
            Id = 1,
            Name = "João Silva",
            Email = "joao@example.com",
            PhoneNumber = "912345678"
        };

        
        var med1 = new Medication
        {
            MedicationId = 1,
            PatientId = 1,
            Name = "Paracetamol 500mg",
            QuantityOnHand = 10,
            QuantityPerUnit = 1,
            LowStockThreshold = 2,
            RequiresPrescription = false
        };

        var med2 = new Medication
        {
            MedicationId = 2,
            PatientId = 1,
            Name = "Amoxicilina 500mg",
            QuantityOnHand = 8,
            QuantityPerUnit = 1,
            LowStockThreshold = 2,
            RequiresPrescription = true
        };

        _context.Patients.Add(patient);
        _context.Medications.AddRange(med1, med2);
        _context.SaveChanges();

        _service = new MedicationPrescriptionService(_context);
        _controller = new PatientMedicationsController(_service);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

  
    [Fact]
    public async Task GetAllForPatient_ReturnsOk_WithList()
    {
        var result = await _controller.GetAllForPatient(1, null);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var meds = Assert.IsAssignableFrom<IEnumerable<Medication>>(ok.Value);
        Assert.True(meds.Count() >= 2);
    }

    
    [Fact]
    public async Task GetAllForPatient_FiltersByPrescription()
    {
        var result = await _controller.GetAllForPatient(1, true);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var meds = Assert.IsAssignableFrom<IEnumerable<Medication>>(ok.Value);
        Assert.All(meds, m => Assert.True(m.RequiresPrescription));
    }

   
    [Fact]
    public async Task GetById_ReturnsOk_WhenExists()
    {
        var result = await _controller.GetById(1, 1);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var med = Assert.IsType<Medication>(ok.Value);
        Assert.Equal(1, med.MedicationId);
        Assert.Equal("Paracetamol 500mg", med.Name);
    }

    
    [Fact]
    public async Task GetById_ReturnsNotFound_WhenMissing()
    {
        var result = await _controller.GetById(1, 99);

        var nf = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Contains("Medicamento não encontrado", nf.Value!.ToString());
    }

    
    [Fact]
    public async Task AddToPatient_ReturnsCreated_WhenValid()
    {
        var newMed = new Medication
        {
            Name = "Ibuprofeno 400mg",
            QuantityOnHand = 5,
            QuantityPerUnit = 1,
            LowStockThreshold = 2,
            RequiresPrescription = false
        };

        var result = await _controller.AddToPatient(1, newMed);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var med = Assert.IsType<Medication>(created.Value);
        Assert.Equal("Ibuprofeno 400mg", med.Name);
        Assert.Equal(1, med.PatientId);
    }

    
    [Fact]
    public async Task UpdateForPatient_ReturnsOk_WhenSuccess()
    {
        var updated = new Medication
        {
            MedicationId = 1,
            PatientId = 1,
            Name = "Paracetamol Atualizado",
            QuantityOnHand = 12,
            QuantityPerUnit = 1,
            LowStockThreshold = 3,
            RequiresPrescription = false
        };

        var result = await _controller.UpdateForPatient(1, 1, updated);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var med = Assert.IsType<Medication>(ok.Value);
        Assert.Equal("Paracetamol Atualizado", med.Name);
    }

    
    [Fact]
    public async Task DeleteForPatient_ReturnsNoContent_WhenSuccess()
    {
        var result = await _controller.DeleteForPatient(1, 1);
        Assert.IsType<NoContentResult>(result);
    }

    
    [Fact]
    public async Task DeleteForPatient_ReturnsNotFound_WhenMissing()
    {
        var result = await _controller.DeleteForPatient(1, 99);

        var nf = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Contains("Medicamento não encontrado", nf.Value!.ToString());
    }
}