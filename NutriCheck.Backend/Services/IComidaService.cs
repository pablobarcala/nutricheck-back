using NutriCheck.Models;

namespace NutriCheck.Backend.Services
{
    public interface IComidaService
    {
        Task<bool> CrearComidaAsync(Comida comida);
        Task<List<Comida>> ObtenerComidasPorNutricionistaAsync(string nutricionistaId);
    }
}
