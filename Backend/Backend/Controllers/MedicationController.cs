using Backend.DTOs.request;
using Backend.Entities;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicationsController : ControllerBase
    {
        private readonly IMedicationService _service;
        
        public MedicationsController(IMedicationService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Medication>>> GetAll()
        {
            var meds = await _service.GetAllAsync();
            return Ok(meds);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Medication>> GetById(int id)
        {
            var med = await _service.GetByIdAsync(id);
            if (med == null)
                return NotFound(new { message = "Medicamento não encontrado." });

            return Ok(med);
        }
        
        [HttpGet("by-patient/{patientId}")]
        public async Task<ActionResult<IEnumerable<Medication>>> GetByPatientId(int patientId)
        {
            var meds = await _service.GetMedicationsByPatientIdAsync(patientId);
            return Ok(meds);
        }

        [HttpPost]
        public async Task<ActionResult<Medication>> Add([FromBody] CreateMedicationRequestDto medicationDto)
        {
            if (medicationDto == null)
                return BadRequest(new { message = "Dados inválidos." });

            var medication = new Medication
            {
                PatientId = medicationDto.PatientId,
                Name = medicationDto.Name,
                QuantityOnHand = medicationDto.QuantityOnHand,
                QuantityPerUnit = medicationDto.QuantityPerUnit,
                LowStockThreshold = medicationDto.LowStockThreshold
            };

            var created = await _service.AddAsync(medication);
            return CreatedAtAction(nameof(GetById), new { id = created.MedicationId }, created);
        }

        [HttpPost("{id}")]
        public async Task<ActionResult<Medication>> UpdateMedication(int id, [FromBody] UpdateMedicationRequestDto updateDto)
        {
            if (updateDto == null)
                return BadRequest(new { message = "Dados inválidos." });

            var medication = new Medication
            {
                PatientId = updateDto.PatientId,
                Name = updateDto.Name,
                QuantityOnHand = updateDto.QuantityOnHand,
                QuantityPerUnit = updateDto.QuantityPerUnit,
                LowStockThreshold = updateDto.LowStockThreshold
            };

            var updated = await _service.UpdateAsync(id, medication);
            if (updated == null)
                return NotFound(new { message = "Medicamento não encontrado." });

            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
                return NotFound(new { message = "Medicamento não encontrado." });

            return NoContent();
        }
    }

    
}