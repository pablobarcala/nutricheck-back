using Microsoft.AspNetCore.Mvc;
using NutriCheck.Backend.Dtos;
using NutriCheck.Backend.Services;

namespace NutriCheck.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<bool>> Register([FromBody] RegistroUserDto user)
        {
            if (user == null)
            {
                return BadRequest("Usuario no válido");
            }

            var response = await _userService.RegistrarUsuarioAsync(user);

            if (!response)
            {
                return BadRequest("El usuario ya existe");
            }

            return Ok("Usuario registrado correctamente");
        }

        //[HttpPost("login")]
        //public IActionResult Login([FromBody] Nutricionista login)
        //{
        //    var usuario = _context.Nutricionistas
        //        .FirstOrDefault(n => n.Email == login.Email && n.Password == login.Password);

        //    if (usuario == null)
        //    {
        //        return Unauthorized("Email o contraseña incorrectos");
        //    }

        //    return Ok($"Bienvenido, {usuario.Nombre}!");
        //}
    }
}
