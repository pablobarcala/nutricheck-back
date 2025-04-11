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

        /// <summary>
        /// Registra una comida consumida por un paciente en un momento del día.
        /// </summary>
        /// <param name="comida">Datos de la comida (paciente, tipo, nombre, calorías, fecha)</param>
        /// <returns>Comida registrada</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<Comida>> RegistrarComida([FromBody] Comida comida)
        {
            _context.Comidas.Add(comida);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(RegistrarComida), new { id = comida.Id }, comida);
        }

        /// <summary>
        /// Devuelve las comidas registradas en una fecha específica, agrupadas por paciente.
        /// </summary>
        /// <param name="fecha">Fecha a consultar (formato: yyyy-MM-dd)</param>
        /// <returns>Lista de comidas agrupadas por paciente para ese día</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
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
