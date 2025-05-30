using NutriCheck.Models;

namespace NutriCheck.Backend.Repositories
{
    public interface IComidaRepository
    {
        Task<bool> CrearComidaAsync(Comida comida);
        Task<List<Comida>> ObtenerComidasPorNutricionistaAsync(string nutricionistaId);
        Task<List<Comida>> ObtenerComidasPorIdsAsync(List<string> ids);
    }
}
