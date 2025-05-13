using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NutriCheck.Backend.Services;
using System.Security.Claims;

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
    }
}
