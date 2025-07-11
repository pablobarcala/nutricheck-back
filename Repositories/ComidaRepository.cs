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

        public async Task<bool> CrearMuchasComidasAsync(List<Comida> comidas)
        {
            try
            {
                await _comidas.InsertManyAsync(comidas);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear muchas comidas: {ex.Message}");
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

        // Método para obtener un listado de comidas por ids
        public async Task<List<Comida>> ObtenerComidasPorIdsAsync(List<string> ids)
        {
            try
            {
                var filter = Builders<Comida>.Filter.In(c => c.Id, ids);
                var comidas = await _comidas.Find(filter).ToListAsync();
                return comidas;
            }
            catch (Exception ex)
            {
                // Manejo de errores
                Console.WriteLine($"Error al obtener las comidas por IDs: {ex.Message}");
                return new List<Comida>();
            }
        }

        // Método para eliminar una comdia
        public async Task<bool> EliminarComidaAsync(string comidaId, string nutricionistaId)
        {
            try
            {
                var resultado = await _comidas.DeleteOneAsync(c => c.Id == comidaId && c.NutricionistaId == nutricionistaId);
                return resultado.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar la comida: {ex.Message}");
                return false;
            }
        }

        // Método para editar una comida
        public async Task<bool> EditarComidaAsync(Comida nuevaComida, string nutricionistaId)
        {
            try
            {
                var resultado = await _comidas.ReplaceOneAsync(
                    c => c.Id == nuevaComida.Id && c.NutricionistaId == nutricionistaId,
                    nuevaComida
                );

                return resultado.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al editar la comida: {ex.Message}");
                return false;
            }
        }
    }
}
