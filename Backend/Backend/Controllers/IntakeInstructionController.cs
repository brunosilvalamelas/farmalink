using Backend.DTOs.request;
using Backend.DTOs.response;
using Backend.Entities;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IntakeInstructionsController : ControllerBase
    {
        private readonly IIntakeInstructionService _service;

        public IntakeInstructionsController(IIntakeInstructionService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IntakeInstructionResponseDto>> GetById(int id)
        {
            var instruction = await _service.GetByIdAsync(id);
            if (instruction == null)
                return NotFound(new { message = "Instrução de toma não encontrada." });

            return Ok(MapToDto(instruction));
        }

        [HttpGet("medication/{medicationId}")]
        public async Task<ActionResult<List<IntakeInstructionResponseDto>>> GetByMedicationId(int medicationId)
        {
            var instructions = await _service.GetByMedicationIdAsync(medicationId);
            return Ok(instructions.Select(MapToDto).ToList());
        }

        [HttpGet("routine/{routineId}")]
        public async Task<ActionResult<List<IntakeInstructionResponseDto>>> GetByRoutineId(int routineId)
        {
            var instructions = await _service.GetByRoutineIdAsync(routineId);
            return Ok(instructions.Select(MapToDto).ToList());
        }

        [HttpGet("patient/{patientId}")]
        public async Task<ActionResult<List<IntakeInstructionResponseDto>>> GetByPatientId(int patientId)
        {
            var instructions = await _service.GetByPatientIdAsync(patientId);
            return Ok(instructions.Select(MapToDto).ToList());
        }

        [HttpPost]
        public async Task<ActionResult<IntakeInstructionResponseDto>> Create([FromBody] CreateIntakeInstructionRequestDto request)
        {
            if (request == null)
                return BadRequest(new { message = "Dados inválidos." });

            var instruction = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = instruction.IntakeInstructionId }, MapToDto(instruction));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<IntakeInstructionResponseDto>> Update(int id, [FromBody] UpdateIntakeInstructionRequestDto request)
        {
            if (request == null)
                return BadRequest(new { message = "Dados inválidos." });

            var instruction = await _service.UpdateAsync(id, request);
            if (instruction == null)
                return NotFound(new { message = "Instrução de toma não encontrada." });

            return Ok(MapToDto(instruction));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
                return NotFound(new { message = "Instrução de toma não encontrada." });

            return NoContent();
        }

        private IntakeInstructionResponseDto MapToDto(IntakeInstruction instruction)
        {
            return new IntakeInstructionResponseDto
            {
                IntakeInstructionId = instruction.IntakeInstructionId,
                MedicationId = instruction.MedicationId,
                MedicationName = instruction.Medication?.Name ?? "Medicamento não encontrado",
                DosePerIntake = instruction.DosePerIntake,
                DosageRegime = instruction.DosageRegime,
                Time = instruction.Time,
                RoutineId = instruction.RoutineId,
                RoutineDescription = instruction.Routine?.Description,
                RoutineTime = instruction.Routine?.Time
            };
        }
    }
}