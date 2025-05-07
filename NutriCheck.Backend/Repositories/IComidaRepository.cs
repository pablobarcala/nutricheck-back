using NutriCheck.Models;

namespace NutriCheck.Backend.Repositories
{
    public interface IComidaRepository
    {
        Task<bool> CrearComidaAsync(Comida comida);
        Task<List<Comida>> ObtenerComidasPorNutricionistaAsync(string nutricionistaId);
    }
}
