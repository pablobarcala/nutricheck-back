using Microsoft.AspNetCore.Mvc;
using NutriCheck.Models;
using NutriCheck.Data;

namespace NutriCheck.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] Nutricionista login)
        {
            var usuario = _context.Nutricionistas
                .FirstOrDefault(n => n.Email == login.Email && n.Password == login.Password);

            if (usuario == null)
            {
                return Unauthorized("Email o contraseña incorrectos");
            }

            return Ok($"Bienvenido, {usuario.Nombre}!");
        }
    }
}
