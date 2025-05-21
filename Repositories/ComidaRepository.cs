using MongoDB.Driver;
using NutriCheck.Models;

namespace NutriCheck.Backend.Repositories
{
    public class ComidaRepository : IComidaRepository
    {
        private readonly IMongoCollection<Comida> _comidas;
        public ComidaRepository(MongoDBConnection connection)
        {
            _comidas = connection.GetCollection<Comida>("comidas");
        }

        // Método para crear una comida
        public async Task<bool> CrearComidaAsync(Comida comida)
        {
            try
            {
                await _comidas.InsertOneAsync(comida);
                return true;
            }
            catch (Exception ex)
            {
                // Manejo de errores
                Console.WriteLine($"Error al crear la comida: {ex.Message}");
                return false;
            }
        }

        // Método para obtener comidas de un nutricionista
        public async Task<List<Comida>> ObtenerComidasPorNutricionistaAsync(string nutricionistaId)
        {
            try
            {
                var comidas = await _comidas.Find(c => c.NutricionistaId == nutricionistaId).ToListAsync();
                return comidas;
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
