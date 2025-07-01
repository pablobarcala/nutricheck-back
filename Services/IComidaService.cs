using NutriCheck.Models;

namespace NutriCheck.Backend.Services
{
    public interface IComidaService
    {
        Task<bool> CrearComidaAsync(Comida comida);
        Task<List<Comida>> ObtenerComidasPorNutricionistaAsync(string nutricionistaId);
        // Eliminar una comida
        Task<bool> EliminarComidaAsync(string comidaId, string nutricionistaId);
        // Editar una comida
        Task<bool> EditarComidaAsync(Comida nuevaComida, string nutricionistaId);
    }
}
