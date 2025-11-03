using Backend.Data;
using Backend.Entities;
using Backend.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BackendTests.Integration.Services
{
    public class MedicationPrescriptionServiceTests : IDisposable
    {
        private readonly DataContext _context;
        private readonly MedicationPrescriptionService _service;

        public MedicationPrescriptionServiceTests()
        {
          
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new DataContext(options);

            
            var patient = new Patient { Id = 1, Name = "João Silva", Email = "joao@example.com", PhoneNumber = "912345678" };

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
                QuantityOnHand = 5,
                QuantityPerUnit = 1,
                LowStockThreshold = 1,
                RequiresPrescription = true
            };

            _context.Patients.Add(patient);
            _context.Medications.AddRange(med1, med2);
            _context.SaveChanges();

            _service = new MedicationPrescriptionService(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

       
        [Fact]
        public async Task GetAllAsync_ReturnsAllMedications()
        {
            var meds = await _service.GetAllAsync(null);

            Assert.NotNull(meds);
            Assert.Equal(2, meds.Count);
        }

        [Fact]
        public async Task GetAllAsync_FiltersByPrescription()
        {
            var meds = await _service.GetAllAsync(true);

            Assert.Single(meds);
            Assert.True(meds[0].RequiresPrescription);
        }

        
        [Fact]
        public async Task GetByPatientIdAsync_ReturnsPatientMeds()
        {
            var meds = await _service.GetByPatientIdAsync(1, null);

            Assert.Equal(2, meds.Count);
            Assert.All(meds, m => Assert.Equal(1, m.PatientId));
        }

        [Fact]
        public async Task GetByPatientIdAsync_FiltersByPrescription()
        {
            var meds = await _service.GetByPatientIdAsync(1, true);

            Assert.Single(meds);
            Assert.Equal("Amoxicilina 500mg", meds.First().Name);
        }

        
        [Fact]
        public async Task GetByIdAsync_ReturnsMedication_WhenExists()
        {
            var med = await _service.GetByIdAsync(1);

            Assert.NotNull(med);
            Assert.Equal("Paracetamol 500mg", med!.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            var med = await _service.GetByIdAsync(999);
            Assert.Null(med);
        }

       
        [Fact]
        public async Task AddAsync_AddsMedicationSuccessfully()
        {
            var newMed = new Medication
            {
                PatientId = 1,
                Name = "Ibuprofeno 400mg",
                QuantityOnHand = 7,
                QuantityPerUnit = 1,
                LowStockThreshold = 2,
                RequiresPrescription = false
            };

            var added = await _service.AddAsync(newMed);

            Assert.NotNull(added);
            Assert.True(added.MedicationId > 0);
            Assert.Equal("Ibuprofeno 400mg", added.Name);
        }

      
        [Fact]
        public async Task AddToPatientAsync_AssociatesWithCorrectPatient()
        {
            var newMed = new Medication
            {
                Name = "Dipirona 500mg",
                QuantityOnHand = 8,
                QuantityPerUnit = 1,
                LowStockThreshold = 3,
                RequiresPrescription = false
            };

            var added = await _service.AddToPatientAsync(1, newMed);

            Assert.Equal(1, added.PatientId);
            Assert.Equal("Dipirona 500mg", added.Name);
        }

        
        [Fact]
        public async Task UpdateAsync_UpdatesFieldsCorrectly()
        {
            var updated = new Medication
            {
                MedicationId = 1,
                PatientId = 1,
                Name = "Paracetamol Atualizado",
                QuantityOnHand = 15,
                QuantityPerUnit = 1,
                LowStockThreshold = 3,
                RequiresPrescription = false
            };

            var result = await _service.UpdateAsync(1, updated);

            Assert.NotNull(result);
            Assert.Equal("Paracetamol Atualizado", result!.Name);
            Assert.Equal(15, result.QuantityOnHand);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNull_WhenNotFound()
        {
            var updated = new Medication
            {
                MedicationId = 999,
                PatientId = 1,
                Name = "Inexistente",
                QuantityOnHand = 5,
                QuantityPerUnit = 1,
                LowStockThreshold = 1,
                RequiresPrescription = false
            };

            var result = await _service.UpdateAsync(999, updated);
            Assert.Null(result);
        }

        
        [Fact]
        public async Task DeleteAsync_RemovesMedication_WhenExists()
        {
            var success = await _service.DeleteAsync(1);
            Assert.True(success);

            var remaining = await _context.Medications.CountAsync();
            Assert.Equal(1, remaining);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsFalse_WhenNotFound()
        {
            var success = await _service.DeleteAsync(999);
            Assert.False(success);
        }
    }
}
