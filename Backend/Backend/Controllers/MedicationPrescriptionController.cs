using Backend.Entities;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/patients/{patientId}/[controller]")]
    public class PatientMedicationsController : ControllerBase
    {
        
        private readonly IMedicationPrescriptionService _prescriptionService;

        
        public PatientMedicationsController(IMedicationPrescriptionService prescriptionService)
        {
            _prescriptionService = prescriptionService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Medication>>> GetAllForPatient(int patientId, [FromQuery] bool? prescribed)
        {
            var meds = await _prescriptionService.GetByPatientIdAsync(patientId, prescribed);
            return Ok(meds);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Medication>> GetById(int patientId, int id)
        {
            var med = await _prescriptionService.GetByIdAsync(id);
            if (med == null || med.PatientId != patientId)
                return NotFound(new { message = "Medicamento não encontrado para este paciente." });

            return Ok(med);
        }

        [HttpPost]
        public async Task<ActionResult<Medication>> AddToPatient(int patientId, [FromBody] Medication medication)
        {
            if (medication == null)
                return BadRequest(new { message = "Dados inválidos." });

            medication.PatientId = patientId;
            var created = await _prescriptionService.AddAsync(medication);
            return CreatedAtAction(nameof(GetById), new { patientId = patientId, id = created.MedicationId }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Medication>> UpdateForPatient(int patientId, int id, [FromBody] Medication medication)
        {
            if (medication == null)
                return BadRequest(new { message = "Dados inválidos." });

            if (medication.PatientId != patientId)
                return BadRequest(new { message = "O paciente associado não corresponde ao medicamento." });

            var updated = await _prescriptionService.UpdateAsync(id, medication);
            if (updated == null)
                return NotFound(new { message = "Medicamento não encontrado para este paciente." });

            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteForPatient(int patientId, int id)
        {
            var med = await _prescriptionService.GetByIdAsync(id);
            if (med == null || med.PatientId != patientId)
                return NotFound(new { message = "Medicamento não encontrado para este paciente." });

            var deleted = await _prescriptionService.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}