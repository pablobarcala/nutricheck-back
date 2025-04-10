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

        [HttpPost]
        public async Task<ActionResult<Paciente>> CrearPaciente(Paciente paciente)
        {
            _context.Pacientes.Add(paciente);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(CrearPaciente), new { id = paciente.Id }, paciente);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Paciente>>> ObtenerPacientes()
        {
            return await Task.FromResult(_context.Pacientes.ToList());
        }
    }
}
