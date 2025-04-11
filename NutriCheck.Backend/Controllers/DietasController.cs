using Microsoft.AspNetCore.Mvc;
using NutriCheck.Models;
using NutriCheck.Data;

namespace NutriCheck.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DietasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DietasController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Asigna un plato a un paciente en una fecha y momento del día.
        /// </summary>
        /// <param name="dieta">Datos de la dieta (paciente, plato, tipo de comida, fecha)</param>
        /// <returns>La dieta asignada o un error si los datos son inválidos</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Dieta>> AsignarDieta([FromBody] Dieta dieta)
        {
            if (dieta.PacienteId <= 0 || dieta.PlatoComidaId <= 0)
                return BadRequest("Paciente y plato deben estar seleccionados.");

            if (string.IsNullOrWhiteSpace(dieta.Tipo))
                return BadRequest("El tipo de comida (desayuno, almuerzo, etc.) es obligatorio.");

            _context.Dietas.Add(dieta);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(AsignarDieta), new { id = dieta.Id }, dieta);
        }

        /// <summary>
        /// Obtiene todas las comidas asignadas a un paciente en una fecha específica.
        /// </summary>
        /// <param name="pacienteId">ID del paciente</param>
        /// <param name="fecha">Fecha de la dieta</param>
        /// <returns>Lista de platos asignados ese día al paciente</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<object>> ObtenerDietasPorPacienteYFecha(
            [FromQuery] int pacienteId,
            [FromQuery] DateTime fecha)
        {
            var dietas = _context.Dietas
                .Where(d => d.PacienteId == pacienteId && d.Fecha.Date == fecha.Date)
                .Select(d => new
                {
                    d.Id,
                    d.Tipo,
                    d.Fecha,
                    d.PacienteId,
                    d.PlatoComidaId,
                    NombrePlato = d.Plato != null ? d.Plato.Nombre : "No disponible",
                    CaloriasAprox = d.Plato != null ? d.Plato.CaloriasAprox : 0,
                    ProteinasAprox = d.Plato != null ? d.Plato.ProteinasAprox : 0
                })
                .ToList();

            return Ok(dietas);
        }
    }
}
