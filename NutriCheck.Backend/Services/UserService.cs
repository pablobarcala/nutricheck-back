using NutriCheck.Backend.Dtos;
using NutriCheck.Backend.Repositories;
using System.Security.Cryptography;
using System.Text;
using NutriCheck.Backend.Models;

namespace NutriCheck.Backend.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // Método para registrar usuario
        public async Task<bool> RegistrarUsuarioAsync(RegistroUserDto user)
        {
            // Verificar si el email ya existe
            if (await _userRepository.UsuarioExisteConMailAsync(user.Email))
            {
                return false; // El usuario ya existe
            }

            CreatePasswordHash(user.Password, out byte[] passwordHash, out byte[] passwordSalt);

            User newUser = new User
            {
                Nombre = user.Nombre,
                Email = user.Email,
                Rol = user.Rol,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
            };

            return await _userRepository.GuardarUsuarioAsync(newUser);
        }

        // Helper method to create password hash and salt
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
