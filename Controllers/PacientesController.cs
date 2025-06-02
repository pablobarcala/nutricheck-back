using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriCheck.Backend.Dtos;
using NutriCheck.Backend.Models;
using NutriCheck.Backend.Services;
using NutriCheck.Models;
using System.Security.Claims;

namespace NutriCheck.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PacientesController : ControllerBase
    {
        private readonly IUserService _userService;
        //private readonly AppDbContext _context;

        public PacientesController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        [HttpPost("guardar-datos")]
        public async Task<ActionResult<string>> GuardarDatosPaciente([FromBody] GuardarDatosPacienteDto datosPaciente)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("El ID de usuario no es válido.");
            }

            if (datosPaciente == null)
            {
                return BadRequest("Los datos del paciente son obligatorios.");
            }

            var response = await _userService.GuardarDatosPaciente(userId, datosPaciente);

            if (!response)
            {
                return BadRequest("Error al guardar los datos del paciente.");
            }

            return Ok("Datos del paciente guardados correctamente.");
        }

        [HttpGet("buscar")]
        public async Task<ActionResult<List<PacienteBuscadoDto>>> BuscarPacientes([FromQuery] string nombre)
        {
            try
            {
                var usuarios = await _userService.BuscarPacientesPorNombre(nombre);
                return Ok(usuarios);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, "Error interno al buscar usuarios");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ocurrió un error inesperado");
            }
        }

        [Authorize(Roles = "paciente")]
        [HttpGet("comidas")]
        public async Task<ActionResult<List<Comida>>> ObtenerComidasDePacienteLoggeado()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                var comidas = await _userService.ObtenerComidasDelPaciente(userId);
                return Ok(comidas);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, "Error interno al buscar comidas");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ocurrió un error inesperado");
            }
        }

        [HttpGet("comidas/{pacienteId}")]
        public async Task<ActionResult<List<Comida>>> ObtenerComidasDelPaciente(string pacienteId)
        {
            try
            {
                var comidas = await _userService.ObtenerComidasDelPaciente(pacienteId);
                return Ok(comidas);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, "Error interno al buscar comidas");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ocurrió un error inesperado");
            }
        }

        [Authorize(Roles = "nutricionista")]
        [HttpPost("agregar-comida/{pacienteId}/{comidaId}")]
        public async Task<ActionResult<bool>> AgregarComidaAPaciente(string pacienteId, string comidaId)
        {
            try
            {
                var response = await _userService.AgregarComidaAPaciente(pacienteId, comidaId);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, "Error interno al agregar comida");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ocurrió un error inesperado");
            }
        }

        [HttpGet("valores-nutricionales/{userId}")]
        public async Task<ActionResult<ValoresNutricionalesDto>> ObtenerValoresNutricionalesPorId(string userId)
        {
            try
            {
                var valoresNutricionales = await _userService.ObtenerValoresNutricionalesDelPaciente(userId);
                return Ok(valoresNutricionales);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, "Error interno al buscar valores nutricionales");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ocurrió un error inesperado");
            }
        }

        [Authorize(Roles = "nutricionista")]
        [HttpPost("editar-valores-nutricionales/{userId}")]
        public async Task<ActionResult<bool>> EditarValoresNutricionalesPorId(string userId, [FromBody] ValoresNutricionalesDto valoresNutricionales)
        {
            try
            {
                var response = await _userService.EditarValoresNutricionales(userId, valoresNutricionales);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, "Error interno al editar valores nutricionales");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ocurrió un error inesperado");
            }
        }

        [HttpGet("paciente/{userId}")]
        public async Task<ActionResult<User>> ObtenerPacientePorId(string userId)
        {
            try
            {
                var response = await _userService.ObtenerUsuarioPorIdAsync(userId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "paciente")]
        [HttpPost("registrar-comida")]
        public async Task<IActionResult> RegistrarComida([FromBody] ComidaRegistrada comida)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                var response = await _userService.RegistrarComidaEnPaciente(userId, comida);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "paciente")]
        [HttpGet("comidas-registradas")]
        public async Task<ActionResult<List<ComidaRegistrada>>> ObtenerComidasRegistradas()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                var response = await _userService.ObtenerComidasRegistradasDePaciente(userId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Crea un nuevo paciente y lo asocia a un nutricionista.
        /// </summary>
        /// <param name="paciente">Datos del paciente</param>
        /// <returns>El paciente creado o un error si los datos son inválidos</returns>
        //[HttpPost]
        //[ProducesResponseType(StatusCodes.Status201Created)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public async Task<ActionResult<Paciente>> CrearPaciente([FromBody] Paciente paciente)
        //{
        //    if (string.IsNullOrWhiteSpace(paciente.Nombre) || paciente.Edad <= 0)
        //        return BadRequest("El nombre y la edad del paciente son obligatorios y válidos.");

        //    _context.Pacientes.Add(paciente);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction(nameof(CrearPaciente), new { id = paciente.Id }, paciente);
        //}

        /// <summary>
        /// Obtiene la lista de todos los pacientes registrados.
        /// </summary>
        /// <returns>Lista de pacientes</returns>
        //[HttpGet]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //public async Task<ActionResult<IEnumerable<Paciente>>> ObtenerPacientes()
        //{
        //    return await Task.FromResult(_context.Pacientes.ToList());
        //}

        /// <summary>
        /// Edita los datos de un paciente existente.
        /// </summary>
        /// <param name="id">ID del paciente a editar</param>
        /// <param name="pacienteActualizado">Nuevos datos del paciente</param>
        /// <returns>Paciente actualizado o error si no se encuentra</returns>
        //[HttpPut("{id}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public async Task<IActionResult> EditarPaciente(int id, [FromBody] Paciente pacienteActualizado)
        //{
        //    var paciente = _context.Pacientes.FirstOrDefault(p => p.Id == id);

        //    if (paciente == null)
        //        return NotFound();

        //    paciente.Nombre = pacienteActualizado.Nombre;
        //    paciente.Edad = pacienteActualizado.Edad;
        //    paciente.Genero = pacienteActualizado.Genero;
        //    paciente.Altura = pacienteActualizado.Altura;
        //    paciente.Peso = pacienteActualizado.Peso;
        //    paciente.Objetivo = pacienteActualizado.Objetivo;
        //    paciente.NutricionistaId = pacienteActualizado.NutricionistaId;

        //    await _context.SaveChangesAsync();

        //    return Ok(paciente);
        //}

        /// <summary>
        /// Elimina un paciente según su ID.
        /// </summary>
        /// <param name="id">ID del paciente a eliminar</param>
        /// <returns>Sin contenido o error si no se encuentra</returns>
        //[HttpDelete("{id}")]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public async Task<IActionResult> EliminarPaciente(int id)
        //{
        //    var paciente = _context.Pacientes.FirstOrDefault(p => p.Id == id);

        //    if (paciente == null)
        //        return NotFound();

        //    _context.Pacientes.Remove(paciente);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}
    }
}
