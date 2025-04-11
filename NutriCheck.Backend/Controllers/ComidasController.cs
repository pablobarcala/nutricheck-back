using Microsoft.AspNetCore.Mvc;
using NutriCheck.Models;
using NutriCheck.Data;
using Microsoft.EntityFrameworkCore;

namespace NutriCheck.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComidasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ComidasController(AppDbContext context)
        {
            _context = context;
        }

        // POST: Registrar comida
        [HttpPost]
        public async Task<ActionResult<Comida>> RegistrarComida(Comida comida)
        {
            _context.Comidas.Add(comida);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(RegistrarComida), new { id = comida.Id }, comida);
        }

        // GET: Obtener comidas por fecha
        [HttpGet]
        public ActionResult<IEnumerable<object>> ObtenerComidasPorFecha([FromQuery] DateTime fecha)

        {
            var comidas = _context.Comidas
                .Where(c => c.Fecha.Date == fecha.Date)
                .Include(c => c.Paciente)
                .ToList();

            var resultado = comidas.Select(c => new
            {
                c.PacienteId,
                NombrePaciente = c.Paciente != null ? c.Paciente.Nombre : "Sin nombre",
                c.Tipo,
                c.Nombre,
                c.Calorias,
                Fecha = c.Fecha.ToShortDateString()
            });

            return Ok(resultado);
        }
    }
}
