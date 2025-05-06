using NutriCheck.Backend.Dtos;

namespace NutriCheck.Backend.Services
{
    public interface IUserService
    {
        Task<bool> RegistrarUsuarioAsync(RegistroUserDto user);
        Task<string> LoginUsuarioAsync(LoginUserDto user);
        Task<bool> GuardarDatosPaciente(string userId, GuardarDatosPacienteDto datosPaciente);
    }
}
