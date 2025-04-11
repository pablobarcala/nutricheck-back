using Microsoft.AspNetCore.Mvc;
using NutriCheck.Models;
using NutriCheck.Data;

namespace NutriCheck.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PacientesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PacientesController(AppDbContext context)
        {
            _context = context;
        }

        // Crear paciente
        [HttpPost]
        public async Task<ActionResult<Paciente>> CrearPaciente(Paciente paciente)
        {
            _context.Pacientes.Add(paciente);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(CrearPaciente), new { id = paciente.Id }, paciente);
        }

        // Listar pacientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Paciente>>> ObtenerPacientes()
        {
            return await Task.FromResult(_context.Pacientes.ToList());
        }

        // Editar paciente
        [HttpPut("{id}")]
        public async Task<IActionResult> EditarPaciente(int id, Paciente pacienteActualizado)
        {
            var paciente = _context.Pacientes.FirstOrDefault(p => p.Id == id);

            if (paciente == null)
            {
                return NotFound();
            }

            paciente.Nombre = pacienteActualizado.Nombre;
            paciente.Edad = pacienteActualizado.Edad;
            paciente.Genero = pacienteActualizado.Genero;
            paciente.Altura = pacienteActualizado.Altura;
            paciente.Peso = pacienteActualizado.Peso;
            paciente.Objetivo = pacienteActualizado.Objetivo;
            paciente.NutricionistaId = pacienteActualizado.NutricionistaId;

            await _context.SaveChangesAsync();

            return Ok(paciente);
        }

        // Eliminar paciente
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarPaciente(int id)
        {
            var paciente = _context.Pacientes.FirstOrDefault(p => p.Id == id);

            if (paciente == null)
            {
                return NotFound();
            }

            _context.Pacientes.Remove(paciente);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
