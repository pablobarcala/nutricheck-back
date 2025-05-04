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

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginUserDto user)
        {
            if (user == null)
            {
                return BadRequest("Usuario necesario");
            }

            var token = await _userService.LoginUsuarioAsync(user);

            if (token == null)
            {
                return Unauthorized("Email o contraseña incorrectos");
            }

            return Ok(token);
        }
    }
}
