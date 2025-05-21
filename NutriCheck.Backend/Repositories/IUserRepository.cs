using NutriCheck.Backend.Dtos;
using NutriCheck.Backend.Models;
using NutriCheck.Models;

namespace NutriCheck.Backend.Repositories
{
    public interface IUserRepository
    {
        Task<bool> GuardarUsuarioAsync(User user);
        Task<bool> UsuarioExisteConMailAsync(string email);
        Task<User?> ObtenerUsuarioPorEmailAsync(string email);
        Task<User?> ObtenerUsuarioPorIdAsync(string id);
        Task<List<User>?> ObtenerPacientesPorNombre(string nombre);
        Task<bool> GuardarDatosPacienteAsync(string id, Paciente datosPaciente);
        Task<bool> EditarUsuarioAsync(User user);
    }
}
