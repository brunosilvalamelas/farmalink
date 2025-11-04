using Backend.Controllers;
using Backend.Data;
using Backend.DTOs.request;
using Backend.Entities;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendTests.Integration.Controllers;

public class MedicationsControllerTests : IDisposable
{
    private readonly DataContext _dataContext;
    private readonly MedicationService _medicationService;
    private readonly MedicationsController _controller;

    public MedicationsControllerTests()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dataContext = new DataContext(options);
        _medicationService = new MedicationService(_dataContext);
        _controller = new MedicationsController(_medicationService);
    }

    public void Dispose()
    {
        _dataContext.Database.EnsureDeleted();
        _dataContext.Dispose();
    }

    private void SeedData()
    {
        var patient = new Patient
        {
            Id = 1,
            Name = "Maria Santos",
            Email = "maria@example.com",
            PhoneNumber = "912345679",
            PasswordHash = "hashed_password",
            Address = "Rua da Maria",
            ZipCode = "1234-568",
            TutorId = 1
        };

        var medication = new Medication
        {
            MedicationId = 1,
            PatientId = 1,
            Name = "Paracetamol",
            QuantityOnHand = 100,
            QuantityPerUnit = 10,
            LowStockThreshold = 20
        };

        _dataContext.Patients.Add(patient);
        _dataContext.Medications.Add(medication);
        _dataContext.SaveChanges();
        _dataContext.ChangeTracker.Clear();
    }

    [Fact]
    public async Task UpdateMedication_ReturnsNotFound_WhenMedicationNotFound()
    {
        // Arrange
        SeedData();
        var nonExistentId = 999;
        var updateDto = new UpdateMedicationRequestDto
        {
            PatientId = 1,
            Name = "Medicamento Inexistente",
            QuantityOnHand = 100,
            QuantityPerUnit = 10,
            LowStockThreshold = 20
        };

        // Act
        var result = await _controller.UpdateMedication(nonExistentId, updateDto);

        // Assert
        var actionResult = Assert.IsType<ActionResult<Medication>>(result);
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        
        // Verifica a mensagem de erro
        var responseObject = notFoundResult.Value;
        var messageProperty = responseObject.GetType().GetProperty("message")?.GetValue(responseObject);
        Assert.Equal("Medicamento não encontrado.", messageProperty);
    }

    [Fact]
    public async Task UpdateMedication_ReturnsBadRequest_WhenDtoIsNull()
    {
        // Arrange
        SeedData();
        var medicationId = 1;
        UpdateMedicationRequestDto updateDto = null;

        // Act
        var result = await _controller.UpdateMedication(medicationId, updateDto);

        // Assert
        var actionResult = Assert.IsType<ActionResult<Medication>>(result);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        
        // Verifica a mensagem de erro
        var responseObject = badRequestResult.Value;
        var messageProperty = responseObject.GetType().GetProperty("message")?.GetValue(responseObject);
        Assert.Equal("Dados inválidos.", messageProperty);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenMedicationNotFound()
    {
        // Arrange
        SeedData();
        var nonExistentId = 999;

        // Act
        var result = await _controller.GetById(nonExistentId);

        // Assert
        var actionResult = Assert.IsType<ActionResult<Medication>>(result);
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        
        // Verifica a mensagem de erro
        var responseObject = notFoundResult.Value;
        var messageProperty = responseObject.GetType().GetProperty("message")?.GetValue(responseObject);
        Assert.Equal("Medicamento não encontrado.", messageProperty);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenMedicationNotFound()
    {
        // Arrange
        SeedData();
        var nonExistentId = 999;

        // Act
        var result = await _controller.Delete(nonExistentId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        
        // Verifica a mensagem de erro
        var responseObject = notFoundResult.Value;
        var messageProperty = responseObject.GetType().GetProperty("message")?.GetValue(responseObject);
        Assert.Equal("Medicamento não encontrado.", messageProperty);
    }

    [Fact]
    public async Task Add_ReturnsBadRequest_WhenDtoIsNull()
    {
        // Arrange
        CreateMedicationRequestDto medicationDto = null;

        // Act
        var result = await _controller.Add(medicationDto);

        // Assert
        var actionResult = Assert.IsType<ActionResult<Medication>>(result);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        
        // Verifica a mensagem de erro
        var responseObject = badRequestResult.Value;
        var messageProperty = responseObject.GetType().GetProperty("message")?.GetValue(responseObject);
        Assert.Equal("Dados inválidos.", messageProperty);
    }

    // Testes que devem passar
    [Fact]
    public async Task GetAll_ReturnsOkResult_WithListOfMedications()
    {
        // Arrange
        SeedData();

        // Act
        var result = await _controller.GetAll();

        // Assert
        var actionResult = Assert.IsType<ActionResult<IEnumerable<Medication>>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var medications = Assert.IsType<List<Medication>>(okResult.Value);
        Assert.Single(medications);
    }

    [Fact]
    public async Task GetById_ReturnsOkResult_WithMedication()
    {
        // Arrange
        SeedData();
        var medicationId = 1;

        // Act
        var result = await _controller.GetById(medicationId);

        // Assert
        var actionResult = Assert.IsType<ActionResult<Medication>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var medication = Assert.IsType<Medication>(okResult.Value);
        Assert.Equal(medicationId, medication.MedicationId);
    }

    [Fact]
    public async Task GetByPatientId_ReturnsOkResult_WithPatientMedications()
    {
        // Arrange
        SeedData();
        var patientId = 1;

        // Act
        var result = await _controller.GetByPatientId(patientId);

        // Assert
        var actionResult = Assert.IsType<ActionResult<IEnumerable<Medication>>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var medications = Assert.IsType<List<Medication>>(okResult.Value);
        Assert.Single(medications);
    }

    [Fact]
    public async Task Add_ReturnsCreatedAtActionResult_WhenMedicationAdded()
    {
        // Arrange
        SeedData();
        var newMedication = new CreateMedicationRequestDto
        {
            PatientId = 1,
            Name = "Novo Medicamento",
            QuantityOnHand = 200,
            QuantityPerUnit = 20,
            LowStockThreshold = 30
        };

        // Act
        var result = await _controller.Add(newMedication);

        // Assert
        var actionResult = Assert.IsType<ActionResult<Medication>>(result);
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
        var medication = Assert.IsType<Medication>(createdAtActionResult.Value);
        Assert.Equal(newMedication.Name, medication.Name);
    }

    [Fact]
    public async Task UpdateMedication_ReturnsOkResult_WhenUpdateSucceeds()
    {
        // Arrange
        SeedData();
        var medicationId = 1;
        var updatedMedication = new UpdateMedicationRequestDto
        {
            PatientId = 1,
            Name = "Paracetamol Atualizado",
            QuantityOnHand = 150,
            QuantityPerUnit = 15,
            LowStockThreshold = 25
        };

        // Act
        var result = await _controller.UpdateMedication(medicationId, updatedMedication);

        // Assert
        var actionResult = Assert.IsType<ActionResult<Medication>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var medication = Assert.IsType<Medication>(okResult.Value);
        Assert.Equal(updatedMedication.Name, medication.Name);
    }

    [Fact]
    public async Task Delete_ReturnsNoContentResult_WhenDeletionSucceeds()
    {
        // Arrange
        SeedData();
        var medicationId = 1;

        // Act
        var result = await _controller.Delete(medicationId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }
}