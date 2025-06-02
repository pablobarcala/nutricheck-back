using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NutriCheck.Backend.Services;
using Nutricheck.Backend.Services; // Este using es para NutricionistaService
using System.Security.Claims;
using System.Threading.Tasks;

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
            var response = await _userService.AgregarPacienteEnNutricionistaAsync(nutricionistaId, pacienteId);
            return Ok(response);
        }

        [Authorize(Roles = "nutricionista")]
        [HttpGet("mis-pacientes")]
        public async Task<IActionResult> ObtenerMisPacientes()
        {
            var nutricionistaId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(nutricionistaId))
                return Unauthorized("Nutricionista no identificado");

            var pacientes = await _userService.ObtenerPacientesDelNutricionista(nutricionistaId);
            return Ok(pacientes);
        }
    }
}
