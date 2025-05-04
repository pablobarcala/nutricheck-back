using NutriCheck.Backend.Dtos;

namespace NutriCheck.Backend.Services
{
    public interface IUserService
    {
        Task<bool> RegistrarUsuarioAsync(RegistroUserDto user);
    }
}
