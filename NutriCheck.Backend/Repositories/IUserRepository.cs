using NutriCheck.Backend.Models;

namespace NutriCheck.Backend.Repositories
{
    public interface IUserRepository
    {
        Task<bool> GuardarUsuarioAsync(User user);
        Task<bool> UsuarioExisteConMailAsync(string email);
    }
}
