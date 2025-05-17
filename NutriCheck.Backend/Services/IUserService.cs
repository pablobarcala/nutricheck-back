using NutriCheck.Backend.Dtos;
using NutriCheck.Backend.Models;

namespace NutriCheck.Backend.Services
{
    public interface IUserService
    {
        Task<bool> RegistrarUsuarioAsync(RegistroUserDto user);
        Task<string> LoginUsuarioAsync(LoginUserDto user);
        Task<bool> GuardarDatosPaciente(string userId, GuardarDatosPacienteDto datosPaciente);
        Task<bool> AgregarPacienteEnNutricionistaAsync(string nutricionistaId, string pacienteId);
        Task<List<User>?> BuscarPacientesPorNombre(string nombre);
    }
}
