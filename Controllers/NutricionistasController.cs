using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NutriCheck.Backend.Services;
using Nutricheck.Backend.Services; // Este using es para NutricionistaService
using System.Security.Claims;
using System.Threading.Tasks;
using NutriCheck.Backend.Dtos;

namespace NutriCheck.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NutricionistasController : ControllerBase
    {
        private readonly IUserService _userService;

        public NutricionistasController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        [HttpPost("agregar-paciente")]
        public async Task<ActionResult<bool>> AgregarPacienteEnNutricionista([FromQuery] string pacienteId)
        {
            var nutricionistaId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine(nutricionistaId);
            var response = await _userService.AgregarPacienteEnNutricionistaAsync(nutricionistaId, pacienteId);
            return Ok(response);
        }

        [Authorize(Roles = "nutricionista")]
        [HttpGet("mis-pacientes")]
        public async Task<IActionResult> ObtenerMisPacientes()
        {
            var nutricionistaId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(nutricionistaId))
                return Unauthorized("Nutricionista no identificado");

            var pacientes = await _userService.ObtenerPacientesDelNutricionista(nutricionistaId);
            return Ok(pacientes);
        }

        [Authorize(Roles = "nutricionista")]
        [HttpGet("comidas-mis-pacientes")]
        public async Task<IActionResult> ObtenerComidasRegistradasDePacientes()
        {
            var nutricionistaId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(nutricionistaId))
                return Unauthorized("Nutricionista no identificado");

            var comidasConPaciente = await _userService.ObtenerComidasRegistradasConInfoPaciente(nutricionistaId);
            return Ok(comidasConPaciente);
        }

        [Authorize(Roles = "nutricionista")]
        [HttpGet("estadisticas-globales")]
        public async Task<ActionResult<EstadisticasGlobalesDto>> CalcularEstadisticasGlobales()
        {
            var nutricionistaId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(nutricionistaId))
                return Unauthorized("Nutricionista no identificado");

            var estadisticas = await _userService.CalcularEstadisticasDeNutricionista(nutricionistaId);
            return Ok(estadisticas);
        }
    }
}
