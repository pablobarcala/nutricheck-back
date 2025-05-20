using MongoDB.Driver;
using NutriCheck.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nutricheck.Backend.Repositories
{
    public class PacienteRepository
    {
        private readonly IMongoCollection<Paciente> _pacientes;

        public PacienteRepository(IMongoDatabase database)
        {
            _pacientes = database.GetCollection<Paciente>("Pacientes"); // Ajustar nombre si tu colección se llama distinto
        }

        public async Task<List<Paciente>> BuscarPacientesPorNutricionistaId(string nutricionistaId)
        {
            var filtro = Builders<Paciente>.Filter.Eq(p => p.NutricionistaId, nutricionistaId);
            return await _pacientes.Find(filtro).ToListAsync();
        }
    }
}
