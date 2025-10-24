using Backend.Entities;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicationsController : ControllerBase
    {
        private readonly MedicationService _service;

        public MedicationsController(MedicationService service)
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

        
        [HttpPut("{id}")]
        public async Task<ActionResult<Medication>> Update(int id, [FromBody] Medication medication)
        {
            if (medication == null)
                return BadRequest(new { message = "Dados inválidos." });

            var updated = await _service.UpdateAsync(id, medication);
            if (updated == null)
                return NotFound(new { message = "Medicamento não encontrado." });

            return Ok(updated);
        }
    }
}