﻿using NutriCheck.Backend.Dtos;
using NutriCheck.Backend.Repositories;
using System.Security.Cryptography;
using System.Text;
using NutriCheck.Backend.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using NutriCheck.Models;

namespace NutriCheck.Backend.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IComidaRepository _comidaRepository;
        private readonly IConfiguration _configuration;
        public UserService(IUserRepository userRepository, IComidaRepository comidaRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _comidaRepository = comidaRepository;
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
                new Claim("role", user.Rol)
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<User> ObtenerUsuarioPorIdAsync(string userId) => await _userRepository.ObtenerUsuarioPorIdAsync(userId);

        public async Task<bool> GuardarDatosPaciente(string userId, GuardarDatosPacienteDto datosPaciente)
        {
            var datos = new NutriCheck.Models.Paciente
            {
                Actividad = datosPaciente.Actividad,
                Altura = datosPaciente.Altura,
                Sexo = datosPaciente.Sexo,
                Peso = datosPaciente.Peso,
                FechaNacimiento = datosPaciente.FechaNacimiento,
                Calorias = datosPaciente.Calorias
            };

            return await _userRepository.GuardarDatosPacienteAsync(userId, datos);
        }

        public async Task<bool> AgregarPacienteEnNutricionistaAsync(string nutricionistaId, string pacienteId)
        {
            var nutricionista = await _userRepository.ObtenerUsuarioPorIdAsync(nutricionistaId);
            var paciente = await _userRepository.ObtenerUsuarioPorIdAsync(pacienteId);

            if (nutricionista is null || paciente is null)
            {
                return false; // El nutricionista no existe
            }

            if (nutricionista.Nutricionista == null)
            {
                nutricionista.Nutricionista = new Nutricionista
                {
                    Pacientes = new List<string>()
                };
            }

            if (paciente.Paciente == null)
            {
                paciente.Paciente = new Paciente();
            }

            if (paciente.Paciente.NutricionistaId != null && paciente.Paciente.NutricionistaId != nutricionistaId)
            {
                return false;
            }

            if (!nutricionista.Nutricionista.Pacientes.Contains(pacienteId))
            {
                nutricionista.Nutricionista.Pacientes.Add(pacienteId);
            }

            paciente.Paciente.NutricionistaId = nutricionistaId;

            var pacienteActualizado = await _userRepository.EditarUsuarioAsync(paciente);
            var nutricionistaActualizado = await _userRepository.EditarUsuarioAsync(nutricionista);

            return pacienteActualizado && nutricionistaActualizado;
        }

        public async Task<List<PacienteBuscadoDto>?> BuscarPacientesPorNombre(string nombre) => await _userRepository.ObtenerPacientesPorNombre(nombre);

        public async Task<List<Comida>> ObtenerComidasDelPaciente(string pacienteId)
        {
            var paciente = await _userRepository.ObtenerUsuarioPorIdAsync(pacienteId);
            if (paciente == null)
            {
                return new List<Comida>();
            }
            var comidas = await _comidaRepository.ObtenerComidasPorIdsAsync(paciente.Paciente.Comidas);
            return comidas;
        }

        public async Task<bool> AgregarComidaAPaciente(string pacienteId, string comidaId)
        {
            var paciente = await _userRepository.ObtenerUsuarioPorIdAsync(pacienteId);
            if (paciente == null)
            {
                return false;
            }
            paciente.Paciente.Comidas.Add(comidaId);
            return await _userRepository.EditarUsuarioAsync(paciente);
        }

        // Método para obtener los valores nutricionales del paciente
        public async Task<ValoresNutricionalesDto> ObtenerValoresNutricionalesDelPaciente(string userId)
        {
            var paciente = await _userRepository.ObtenerUsuarioPorIdAsync(userId);
            if (paciente == null)
            {
                return null; // El paciente no existe
            }
            var valoresNutricionales = new ValoresNutricionalesDto
            {
                Calorias = paciente.Paciente.Calorias,
                Grasas = paciente.Paciente.Grasas,
                Carbohidratos = paciente.Paciente.Carbohidratos,
                Proteinas = paciente.Paciente.Proteinas
            };
            return valoresNutricionales;
        }

        // Método para editar los valores nutricionales del paciente
        public async Task<bool> EditarValoresNutricionales(string userId, ValoresNutricionalesDto valoresNutricionales)
        {
            var paciente = await _userRepository.ObtenerUsuarioPorIdAsync(userId);
            if (paciente == null)
            {
                return false; // El paciente no existe
            }
            paciente.Paciente.Calorias = valoresNutricionales.Calorias;
            paciente.Paciente.Grasas = valoresNutricionales.Grasas;
            paciente.Paciente.Carbohidratos = valoresNutricionales.Carbohidratos;
            paciente.Paciente.Proteinas = valoresNutricionales.Proteinas;
            return await _userRepository.EditarUsuarioAsync(paciente);
        }

        // Método para traer pacientes del nutricionista
        public async Task<List<User>> ObtenerPacientesDelNutricionista(string nutricionistaId)
        {
            var nutricionista = await _userRepository.ObtenerUsuarioPorIdAsync(nutricionistaId);

            // Validación básica
            if (nutricionista?.Nutricionista?.Pacientes == null || !nutricionista.Nutricionista.Pacientes.Any())
            {
                return new List<User>();
            }

            // Paso 2: obtener la lista de IDs de pacientes
            var pacientesIds = nutricionista.Nutricionista.Pacientes;

            // Paso 3: buscar todos los usuarios que tengan esos IDs
            var usuariosPacientes = await _userRepository.ObtenerUsuariosPorIdsAsync(pacientesIds);

            // Paso 4: obtener solo la propiedad Paciente de cada User (y filtrar nulos por seguridad)
            var pacientes = usuariosPacientes
                .Where(u => u.Paciente != null)
                .Select(u => u)
                .ToList();

            return pacientes;
        }

        public async Task<List<ComidaRegistrada>> ObtenerComidasRegistradasDePaciente(string userId)
        {
            var paciente = await _userRepository.ObtenerUsuarioPorIdAsync(userId);
            
            return paciente.Paciente.ComidasRegistradas;
        }

        public async Task<List<ComidaRegistradaConPacienteDto>> ObtenerComidasRegistradasConInfoPaciente(string nutricionistaId)
        {
            var pacientes = await ObtenerPacientesDelNutricionista(nutricionistaId);

            if (pacientes == null || !pacientes.Any())
            {
                return new List<ComidaRegistradaConPacienteDto>();
            }

            var comidasConPaciente = new List<ComidaRegistradaConPacienteDto>();

            foreach (var paciente in pacientes)
            {
                var pacienteDto = new ComidaRegistradaConPacienteDto
                {
                    Nombre = paciente.Nombre ?? string.Empty // o como tengas el nombre en tu modelo
                };

                if (paciente.Paciente?.ComidasRegistradas != null && paciente.Paciente.ComidasRegistradas.Any())
                {
                    foreach (var comida in paciente.Paciente.ComidasRegistradas)
                    {
                        if (comida != null)
                        {
                            pacienteDto.Comidas.Add(new ComidaParaFront
                            {
                                Title = comida.Nombre ?? string.Empty,
                                Horario = comida.Horario ?? string.Empty,
                                Fecha = comida.Fecha ?? string.Empty
                            });
                        }
                    }
                }

                comidasConPaciente.Add(pacienteDto);
            }

            return comidasConPaciente;
        }

        public async Task<bool> RegistrarComidaEnPaciente(string userId, ComidaRegistrada comida)
        {
            var paciente = await _userRepository.ObtenerUsuarioPorIdAsync(userId);
            if (paciente == null)
            {
                return false;
            }

            paciente.Paciente.ComidasRegistradas.Add(comida);
            return await _userRepository.EditarUsuarioAsync(paciente);
        }
    }
}
