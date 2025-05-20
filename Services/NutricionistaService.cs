using Nutricheck.Backend.Repositories;
using NutriCheck.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nutricheck.Backend.Services
{
    public class NutricionistaService
    {
        private readonly PacienteRepository _pacienteRepository;

        public NutricionistaService(PacienteRepository pacienteRepository)
        {
            _pacienteRepository = pacienteRepository;
        }

        public async Task<List<Paciente>> ObtenerPacientesDelNutricionista(string nutricionistaId)
        {
            return await _pacienteRepository.BuscarPacientesPorNutricionistaId(nutricionistaId);
        }
    }
}
