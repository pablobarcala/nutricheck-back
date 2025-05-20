using NutriCheck.Backend.Repositories;
using NutriCheck.Models;

namespace NutriCheck.Backend.Services
{
    public class ComidaService : IComidaService
    {
        private readonly IComidaRepository _comidaRepository;

        public ComidaService(IComidaRepository comidaRepository)
        {
            _comidaRepository = comidaRepository;
        }

        public async Task<bool> CrearComidaAsync(Comida comida)
        {
            try
            {
                return await _comidaRepository.CrearComidaAsync(comida);
            }
            catch (Exception ex)
            {
                // Manejo de errores
                Console.WriteLine($"Error al crear la comida: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Comida>> ObtenerComidasPorNutricionistaAsync(string nutricionistaId)
        {
            try
            {
                return await _comidaRepository.ObtenerComidasPorNutricionistaAsync(nutricionistaId);
            }
            catch (Exception ex)
            {
                // Manejo de errores
                Console.WriteLine($"Error al obtener las comidas: {ex.Message}");
                return new List<Comida>();
            }
        }
    }
}
