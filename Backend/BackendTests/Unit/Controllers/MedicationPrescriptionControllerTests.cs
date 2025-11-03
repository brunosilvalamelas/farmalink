using Backend.Controllers;
using Backend.Entities;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BackendTests.Unit.Controllers;

public class PatientMedicationsControllerTests
{
    private readonly Mock<IMedicationPrescriptionService> _mockService;
    private readonly PatientMedicationsController _controller;

    public PatientMedicationsControllerTests()
    {
        _mockService = new Mock<IMedicationPrescriptionService>();
        _controller = new PatientMedicationsController(_mockService.Object);
    }

   
    [Fact]
    public async Task GetAllForPatient_ReturnsOk_WithList()
    {
        
        var meds = new List<Medication>
        {
            new Medication { MedicationId = 1, Name = "Paracetamol", PatientId = 1, RequiresPrescription = false },
            new Medication { MedicationId = 2, Name = "Amoxicilina", PatientId = 1, RequiresPrescription = true }
        };

        _mockService.Setup(s => s.GetByPatientIdAsync(1, null)).ReturnsAsync(meds);

        // Act
        var result = await _controller.GetAllForPatient(1, null);

        // Assert
        var action = Assert.IsType<ActionResult<IEnumerable<Medication>>>(result);
        var ok = Assert.IsType<OkObjectResult>(action.Result);
        var list = Assert.IsType<List<Medication>>(ok.Value);
        Assert.Equal(2, list.Count);
    }

 
    [Fact]
    public async Task GetById_ReturnsOk_WhenFound()
    {
        var med = new Medication { MedicationId = 1, Name = "Ibuprofeno", PatientId = 1 };
        _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(med);

        var result = await _controller.GetById(1, 1);

        var action = Assert.IsType<ActionResult<Medication>>(result);
        var ok = Assert.IsType<OkObjectResult>(action.Result);
        var value = Assert.IsType<Medication>(ok.Value);
        Assert.Equal("Ibuprofeno", value.Name);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenMissing()
    {
        _mockService.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((Medication?)null);

        var result = await _controller.GetById(1, 99);

        var nf = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Contains("Medicamento não encontrado", nf.Value!.ToString());
    }

    
    [Fact]
    public async Task AddToPatient_ReturnsCreatedAtAction_WhenValid()
    {
        var med = new Medication
        {
            MedicationId = 1,
            Name = "Brufen",
            PatientId = 1,
            RequiresPrescription = false
        };

        _mockService.Setup(s => s.AddAsync(It.IsAny<Medication>())).ReturnsAsync(med);

        var result = await _controller.AddToPatient(1, med);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var value = Assert.IsType<Medication>(created.Value);
        Assert.Equal("Brufen", value.Name);
    }

    [Fact]
    public async Task AddToPatient_ReturnsBadRequest_WhenNull()
    {
        var result = await _controller.AddToPatient(1, null!);

        var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Contains("Dados inválidos", bad.Value!.ToString());
    }

    // ✅ PUT /api/patients/{patientId}/medications/{id}
    [Fact]
    public async Task UpdateForPatient_ReturnsOk_WhenValid()
    {
        var med = new Medication { MedicationId = 1, PatientId = 1, Name = "Paracetamol Atualizado" };
        _mockService.Setup(s => s.UpdateAsync(1, med)).ReturnsAsync(med);

        var result = await _controller.UpdateForPatient(1, 1, med);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var value = Assert.IsType<Medication>(ok.Value);
        Assert.Equal("Paracetamol Atualizado", value.Name);
    }

    [Fact]
    public async Task UpdateForPatient_ReturnsBadRequest_WhenMismatch()
    {
        var med = new Medication { MedicationId = 1, PatientId = 2, Name = "Erro" };

        var result = await _controller.UpdateForPatient(1, 1, med);

        var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Contains("paciente associado", bad.Value!.ToString());
    }

    [Fact]
    public async Task UpdateForPatient_ReturnsNotFound_WhenMissing()
    {
        var med = new Medication { MedicationId = 99, PatientId = 1, Name = "Desconhecido" };
        _mockService.Setup(s => s.UpdateAsync(99, med)).ReturnsAsync((Medication?)null);

        var result = await _controller.UpdateForPatient(1, 99, med);

        var nf = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Contains("Medicamento não encontrado", nf.Value!.ToString());
    }

    
    [Fact]
    public async Task DeleteForPatient_ReturnsNoContent_WhenSuccessful()
    {
        var med = new Medication { MedicationId = 1, PatientId = 1 };
        _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(med);
        _mockService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

        var result = await _controller.DeleteForPatient(1, 1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteForPatient_ReturnsNotFound_WhenMissing()
    {
        _mockService.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((Medication?)null);

        var result = await _controller.DeleteForPatient(1, 99);

        var nf = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Contains("Medicamento não encontrado", nf.Value!.ToString());
    }
}
