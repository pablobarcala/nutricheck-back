using NutriCheck.Backend.Dtos;
using NutriCheck.Backend.Repositories;
using System.Security.Cryptography;
using System.Text;
using NutriCheck.Backend.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace NutriCheck.Backend.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        public UserService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
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

        // Método para iniciar sesión
        public async Task<string> LoginUsuarioAsync(LoginUserDto user)
        {
            // Verificar si el usuario existe
            var existingUser = await _userRepository.ObtenerUsuarioPorEmailAsync(user.Email);
            if (existingUser == null)
            {
                return null; // El usuario no existe
            }

            // Verificar la contraseña
            if (!VerifyPasswordHash(user.Password, existingUser.PasswordHash, existingUser.PasswordSalt))
            {
                return null; // Contraseña incorrecta
            }

            return GenerateJwtToken(existingUser); // Inicio de sesión exitoso
        }

        // Helper method to verify password hash
        private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using (var hmac = new HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(storedHash);
            }
        }

        // Helper method to generate JWT token
        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Token"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim("email", user.Email),
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
