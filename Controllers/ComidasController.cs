using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NutriCheck.Backend.Services;
using NutriCheck.Models;
using System.Security.Claims;

namespace NutriCheck.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComidasController : ControllerBase
    {
        private readonly IComidaService _comidaService;

        public ComidasController(IComidaService comidaService)
        {
            _comidaService = comidaService;
        }

        [Authorize(Roles = "nutricionista")]
        [HttpPost("crear")]
        public async Task<ActionResult<string>> CrearComida([FromBody] Comida comida)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            comida.NutricionistaId = userId;

            if (comida == null)
            {
                return BadRequest("La comida no puede ser nula.");
            }
            try
            {
                var resultado = await _comidaService.CrearComidaAsync(comida);
                if (resultado)
                {
                    return Ok("Comida creada exitosamente.");
                }
                else
                {
                    return StatusCode(500, "Error al crear la comida.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [Authorize(Roles = "nutricionista")]
        [HttpPost("crear-multiples")]
        public async Task<ActionResult<string>> CrearMuchasComidas([FromBody] List<Comida> comidas)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            comidas.ForEach(c => c.NutricionistaId = userId);

            if (comidas.IsNullOrEmpty())
            {
                return BadRequest("La lista de comidas es nula");
            }
            try
            {
                var resultado = await _comidaService.CrearMuchasComidasAsync(comidas);
                if (resultado)
                {
                    return Ok("Comidas creadas exitosamente");
                }
                else
                {
                    return StatusCode(500, "Error al crear las comidas");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno {ex.Message}");
            }
        }

        [Authorize(Roles = "nutricionista")]
        [HttpGet("comidas-de-nutricionista")]
        public async Task<ActionResult<string>> VerComidasDeNutricionista()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("El ID de usuario no es válido.");
            }

            try
            {
                var comidas = await _comidaService.ObtenerComidasPorNutricionistaAsync(userId);

                return Ok(comidas);
            } catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [Authorize(Roles = "nutricionista")]
        [HttpDelete("eliminar/{comidaId}")]
        public async Task<ActionResult<bool>> EliminarComida(string comidaId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Usuario no encontrado");
            }

            try
            {
                var response = await _comidaService.EliminarComidaAsync(comidaId, userId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [Authorize(Roles = "nutricionista")]
        [HttpPut("editar")]
        public async Task<ActionResult<bool>> EditarComida([FromBody] Comida nuevaComida)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty (userId))
            {
                return BadRequest("Usuario no encontrado");
            }

            try
            {
                var response = await _comidaService.EditarComidaAsync(nuevaComida, userId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Registra una comida consumida por un paciente en un momento del día.
        /// </summary>
        /// <param name="comida">Datos de la comida (paciente, tipo, nombre, calorías, fecha)</param>
        /// <returns>Comida registrada</returns>
        //[HttpPost]
        //[ProducesResponseType(StatusCodes.Status201Created)]
        //public async Task<ActionResult<Comida>> RegistrarComida([FromBody] Comida comida)
        //{
        //    _context.Comidas.Add(comida);
        //    await _context.SaveChangesAsync();
        //    return CreatedAtAction(nameof(RegistrarComida), new { id = comida.Id }, comida);
        //}

        /// <summary>
        /// Devuelve las comidas registradas en una fecha específica, agrupadas por paciente.
        /// </summary>
        /// <param name="fecha">Fecha a consultar (formato: yyyy-MM-dd)</param>
        /// <returns>Lista de comidas agrupadas por paciente para ese día</returns>
        //[HttpGet]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //public ActionResult<IEnumerable<object>> ObtenerComidasPorFecha([FromQuery] DateTime fecha)
        //{
        //    var comidas = _context.Comidas
        //        .Where(c => c.Fecha.Date == fecha.Date)
        //        .Include(c => c.Paciente)
        //        .ToList();

        //    var resultado = comidas.Select(c => new
        //    {
        //        c.PacienteId,
        //        NombrePaciente = c.Paciente != null ? c.Paciente.Nombre : "Sin nombre",
        //        c.Tipo,
        //        c.Nombre,
        //        c.Calorias,
        //        Fecha = c.Fecha.ToShortDateString()
        //    });

        //    return Ok(resultado);
        //}

        /// <summary>
        /// Devuelve las comidas faltantes para un paciente en una fecha específica.
        /// Los tipos de comidas esperados son: Desayuno, Almuerzo, Merienda y Cena.
        /// </summary>
        /// <param name="pacienteId">ID del paciente para el cual se consultarán las comidas faltantes</param>
        /// <param name="fecha">Fecha a consultar (formato: yyyy-MM-dd)</param>
        /// <returns>Lista de tipos de comidas faltantes para ese paciente y fecha</returns>
        //[HttpGet("faltantes")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //public ActionResult<IEnumerable<string>> ObtenerComidasFaltantes(
        //    [FromQuery] int pacienteId, [FromQuery] DateTime fecha)
        //{
        //    var tiposEsperados = new[] { "Desayuno", "Almuerzo", "Merienda", "Cena" };

        //    var tiposRegistrados = _context.Comidas
        //        .Where(c => c.PacienteId == pacienteId && c.Fecha.Date == fecha.Date)
        //        .Select(c => c.Tipo)
        //        .ToList();

        //    var faltantes = tiposEsperados
        //        .Where(tipo => !tiposRegistrados.Contains(tipo, StringComparer.OrdinalIgnoreCase))
        //        .ToList();

        //    return Ok(faltantes);
        //}
    }
}
