using Microsoft.AspNetCore.Mvc;
using NutriCheck.Models;
using NutriCheck.Data;

namespace NutriCheck.Controllers
{
    [ApiController]
    [Route("api/nutricionista/comidas")]
    public class PlatosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PlatosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("crear")]
        public async Task<ActionResult<PlatoComida>> CrearPlato([FromBody] PlatoComida plato)
        {
            // Por ahora no validamos nutricionista logueado, asumimos que el ID es válido
            _context.PlatosComida.Add(plato);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(CrearPlato), new { id = plato.Id }, plato);
        }
    }
}
