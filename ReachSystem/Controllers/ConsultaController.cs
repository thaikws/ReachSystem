using Microsoft.AspNetCore.Mvc;
using ReachSystem.Models;
using ReachSystem.Services;

namespace ReachSystem.Controllers
{
    public class ConsultaController : ControllerBase
    {
        private readonly ConsultaService _service;

        public ConsultaController(ConsultaService service)
        {
            _service = service;
        }

        // GET
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var consultas = await _service.GetAllConsultasAsync();
            return Ok(consultas);
        }

        // GET por ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var consulta = await _service.GetConsultaByIdAsync(id);

            if (consulta == null)
                return NotFound();

            return Ok(consulta);
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Consulta consulta)
        {
            try
            {
                var novaConsulta = await _service.AddConsultaAsync(consulta);
                return CreatedAtAction(nameof(GetById), new { id = novaConsulta.ConsultaID }, novaConsulta);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Consulta consulta)
        {
            if (id != consulta.ConsultaID)
                return BadRequest("ID inconsistente");

            var atualizado = await _service.UpdateConsultaAsync(consulta);

            if (!atualizado)
                return NotFound();

            return NoContent();
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deletado = await _service.DeleteConsultaAsync(id);

            if (!deletado)
                return NotFound();

            return NoContent();
        }
    }
}
