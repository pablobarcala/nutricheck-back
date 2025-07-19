using NutriCheck.Backend.Dtos;
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
                new Claim("nombre", user.Nombre),
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
            var datos = new Paciente
            {
                Actividad = datosPaciente.Actividad,
                Altura = datosPaciente.Altura,
                Sexo = datosPaciente.Sexo,
                Peso = datosPaciente.Peso,
                FechaNacimiento = datosPaciente.FechaNacimiento,
                Calorias = datosPaciente.Calorias,
                PlanSemanal = new List<PlanSemanal>()
                {
                    new() {
                        Dia = "Lunes",
                        Comidas = new List<string>()
                    },
                    new() {
                        Dia = "Martes",
                        Comidas = new List<string>()
                    },
                    new() {
                        Dia = "Miércoles",
                        Comidas = new List<string>()
                    },
                    new() {
                        Dia = "Jueves",
                        Comidas = new List<string>()
                    },
                    new() {
                        Dia = "Viernes",
                        Comidas = new List<string>()
                    },
                    new() {
                        Dia = "Sábado",
                        Comidas = new List<string>()
                    },
                    new() {
                        Dia = "Domingo",
                        Comidas = new List<string>()
                    },
                }
            };

            return await _userRepository.GuardarDatosPacienteAsync(userId, datos);
        }

        public async Task<bool> AgregarPacienteEnNutricionistaAsync(string nutricionistaId, string pacienteId)
        {
            var nutricionista = await _userRepository.ObtenerUsuarioPorIdAsync(nutricionistaId);
            var paciente = await _userRepository.ObtenerUsuarioPorIdAsync(pacienteId);

            Console.WriteLine(nutricionista.Nombre);
            Console.WriteLine(paciente.Nombre);

            if (nutricionista is null || paciente is null)
            {
                Console.WriteLine("El nutricionista o paciente no existe");
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
                Console.WriteLine("El paciente ya está vinculado a otro nutricionista");
                return false;
            }

            if (!nutricionista.Nutricionista.Pacientes.Contains(pacienteId))
            {
                nutricionista.Nutricionista.Pacientes.Add(pacienteId);
            }

            paciente.Paciente.NutricionistaId = nutricionistaId;

            var pacienteActualizado = await _userRepository.EditarUsuarioAsync(paciente);
            var nutricionistaActualizado = await _userRepository.EditarUsuarioAsync(nutricionista);

            Console.WriteLine("Paciente actualizado: ", pacienteActualizado);
            Console.WriteLine("Nutricionista actualizado: ", nutricionistaActualizado);

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

        // METODO PARA DEVOLVER ESTADISTICAS DE UN NUTRICIONISTA
        public async Task<EstadisticasGlobalesDto> CalcularEstadisticasDeNutricionista(string nutricionistaId)
        {
            var pacientes = await ObtenerPacientesDelNutricionista(nutricionistaId);

            var promedioGlobal = await CalcularPromedioCumplimientoCalorico(pacientes);
            var cumplimientoPorDia = await CalcularCumplimientoDiario(pacientes);
            var pacientesBajo = await CalcularPacientesConBajoCumplimiento(pacientes);
            var comidasPopulares = CalcularComidasMasRegistradas(pacientes);
            var nivelesActividad = CalcularPacientesPorNivelActividad(pacientes);

            var ranking = CalcularRankingDePacientes(pacientes);
            var pacientesConstantes = ranking.OrderByDescending(r => r.DiasConRegistro).Take(5).ToList();
            var pacientesFaltantes = ranking.OrderBy(r => r.DiasConRegistro).Take(5).ToList();

            var estadisticas = new EstadisticasGlobalesDto
            {
                PromedioCumplimientoCalorico = promedioGlobal,
                PacientesConBajoCumplimiento = pacientesBajo,
                CumplimientoCaloricoPorDia = cumplimientoPorDia,
                ComidasMasPopulares = comidasPopulares,
                PacientesPorNivelActividad = nivelesActividad,
                PacientesMasConstantes = pacientesConstantes,
                PacientesConMasFaltantes = pacientesFaltantes
            };

            return estadisticas;
        }

        // METODOS DE ESTADISTICAS
        private async Task<double> CalcularPromedioCumplimientoCalorico(List<User> pacientes)
        {
            var fechaLimite = DateTime.UtcNow.Date.AddDays(-6);

            double promedioTotal = 0.0;
            int pacientesContados = 0;

            foreach (var paciente in pacientes)
            {
                var caloriasRecomendadas = paciente.Paciente.Calorias;
                if (caloriasRecomendadas == 0 || paciente.Paciente.ComidasRegistradas == null)
                    continue;

                // Obtener comidas registradas de los últimos 7 días
                var comidasUltimaSemana = paciente.Paciente.ComidasRegistradas
                    .Where(c =>
                        !string.IsNullOrEmpty(c.Fecha) &&
                        DateTime.TryParse(c.Fecha, out var fechaParseada) &&
                        fechaParseada.Date >= fechaLimite)
                    .ToList();

                //Console.WriteLine(comidasUltimaSemana);

                if (!comidasUltimaSemana.Any()) continue;

                // Obtener los IDs únicos de comidas
                var comidaIds = comidasUltimaSemana
                    .Select(c => c.ComidaId)
                    .Where(id => !string.IsNullOrEmpty(id))
                    .Distinct()
                    .ToList();

                //Console.WriteLine(comidaIds);

                if (!comidaIds.Any()) continue;

                // Obtener las comidas con sus calorías desde la colección central
                var comidasConCalorias = await _comidaRepository.ObtenerComidasPorIdsAsync(comidaIds);
                var dictCalorias = comidasConCalorias.ToDictionary(c => c.Id, c => c.Kcal);

                // Agrupar las comidas por día y sumar las calorías
                var caloriasPorDia = comidasUltimaSemana
                    .GroupBy(c => DateTime.Parse(c.Fecha).Date)
                    .Select(g => new
                    {
                        Fecha = g.Key,
                        TotalCalorias = g.Sum(c => dictCalorias.GetValueOrDefault(c.ComidaId, 0))
                    })
                    .ToList();

                if (!caloriasPorDia.Any()) continue;

                // Calcular promedio de cumplimiento por día
                var promedioCumplimiento = caloriasPorDia
                    .Average(dia => (double)dia.TotalCalorias / caloriasRecomendadas * 100);

                promedioTotal += promedioCumplimiento;
                pacientesContados++;
            }

            return pacientesContados == 0 ? 0 : promedioTotal / pacientesContados;
        }

        private async Task<List<CumplimientoDiarioDto>> CalcularCumplimientoDiario(List<User> pacientes)
        {
            var dias = Enumerable.Range(0, 7)
                .Select(offset => DateTime.UtcNow.Date.AddDays(-offset))
                .OrderBy(d => d)
                .ToList();

            var resultado = new List<CumplimientoDiarioDto>();

            foreach (var dia in dias)
            {
                double sumaCumplimiento = 0;
                int pacientesContados = 0;

                foreach (var paciente in pacientes)
                {
                    var caloriasRecomendadas = paciente.Paciente?.Calorias ?? 0;
                    if (caloriasRecomendadas == 0 || paciente.Paciente.ComidasRegistradas == null)
                        continue;

                    var comidasDelDia = paciente.Paciente.ComidasRegistradas
                        .Where(c =>
                            !string.IsNullOrEmpty(c.Fecha) &&
                            DateTime.TryParse(c.Fecha, out var fechaParseada) &&
                            fechaParseada.Date == dia)
                        .ToList();

                    if (!comidasDelDia.Any()) continue;

                    var comidaIds = comidasDelDia
                        .Select(c => c.ComidaId)
                        .Where(id => !string.IsNullOrEmpty(id))
                        .Distinct()
                        .ToList();

                    if (!comidaIds.Any()) continue;

                    var comidasConCalorias = await _comidaRepository.ObtenerComidasPorIdsAsync(comidaIds);
                    var dictCalorias = comidasConCalorias.ToDictionary(c => c.Id, c => c.Kcal);

                    int kcalConsumidas = comidasDelDia.Sum(c => dictCalorias.GetValueOrDefault(c.ComidaId, 0));
                    double cumplimiento = (double)kcalConsumidas / caloriasRecomendadas * 100;

                    sumaCumplimiento += cumplimiento;
                    pacientesContados++;
                }

                double promedioDia = pacientesContados > 0 ? sumaCumplimiento / pacientesContados : 0;

                resultado.Add(new CumplimientoDiarioDto
                {
                    Fecha = dia,
                    PorcentajeCumplido = promedioDia
                });
            }

            return resultado;
        }

        private async Task<int> CalcularPacientesConBajoCumplimiento(List<User> pacientes)
        {
            var dias = Enumerable.Range(0, 7)
                .Select(offset => DateTime.UtcNow.Date.AddDays(-offset))
                .ToList();

            int pacientesConBajoCumplimiento = 0;

            foreach (var paciente in pacientes)
            {
                var caloriasRecomendadas = paciente.Paciente?.Calorias ?? 0;
                if (caloriasRecomendadas == 0 || paciente.Paciente.ComidasRegistradas == null)
                    continue;

                int diasBajo = 0;

                foreach (var dia in dias)
                {
                    var comidasDelDia = paciente.Paciente.ComidasRegistradas
                        .Where(c =>
                            !string.IsNullOrEmpty(c.Fecha) &&
                            DateTime.TryParse(c.Fecha, out var fechaParseada) &&
                            fechaParseada.Date == dia)
                        .ToList();

                    if (!comidasDelDia.Any()) continue;

                    var comidaIds = comidasDelDia
                        .Select(c => c.ComidaId)
                        .Where(id => !string.IsNullOrEmpty(id))
                        .Distinct()
                        .ToList();

                    if (!comidaIds.Any()) continue;

                    var comidasConCalorias = await _comidaRepository.ObtenerComidasPorIdsAsync(comidaIds);
                    var dictCalorias = comidasConCalorias.ToDictionary(c => c.Id, c => c.Kcal);

                    int kcalConsumidas = comidasDelDia.Sum(c => dictCalorias.GetValueOrDefault(c.ComidaId, 0));
                    double cumplimiento = (double)kcalConsumidas / caloriasRecomendadas * 100;

                    if (cumplimiento < 80)
                        diasBajo++;
                }

                if (diasBajo >= 3)
                    pacientesConBajoCumplimiento++;
            }

            return pacientesConBajoCumplimiento;
        }

        private List<ComidaPopularDto> CalcularComidasMasRegistradas(List<User> pacientes)
        {
            var fechaLimite = DateTime.UtcNow.Date.AddDays(-6);

            var comidas = pacientes
                .Where(p => p.Paciente?.ComidasRegistradas != null)
                .SelectMany(p => p.Paciente.ComidasRegistradas)
                .Where(c =>
                    !string.IsNullOrEmpty(c.Fecha) &&
                    DateTime.TryParse(c.Fecha, out var fechaParseada) &&
                    fechaParseada.Date >= fechaLimite &&
                    !string.IsNullOrEmpty(c.Nombre))
                .ToList();

            var topComidas = comidas
                .GroupBy(c => c.Nombre!.Trim().ToLower())
                .Select(g => new ComidaPopularDto
                {
                    Nombre = g.First().Nombre,
                    Cantidad = g.Count()
                })
                .OrderByDescending(c => c.Cantidad)
                .Take(5)
                .ToList();

            return topComidas;
        }

        private List<NivelActividadDto> CalcularPacientesPorNivelActividad(List<User> pacientes)
        {
            return pacientes
                .Where(p => p.Paciente != null && !string.IsNullOrEmpty(p.Paciente.Actividad))
                .GroupBy(p => p.Paciente.Actividad!.Trim().ToLower())
                .Select(g => new NivelActividadDto
                {
                    Nivel = g.Key,
                    Cantidad = g.Count()
                })
                .ToList();
        }

        private List<RankingPacienteDto> CalcularRankingDePacientes(List<User> pacientes)
        {
            var dias = Enumerable.Range(0, 7)
                .Select(offset => DateTime.UtcNow.Date.AddDays(-offset))
                .ToList();

            var ranking = new List<RankingPacienteDto>();

            foreach (var paciente in pacientes)
            {
                if (paciente.Paciente?.ComidasRegistradas == null) continue;

                var diasConRegistro = dias.Count(dia =>
                    paciente.Paciente.ComidasRegistradas.Any(c =>
                        !string.IsNullOrEmpty(c.Fecha) &&
                        DateTime.TryParse(c.Fecha, out var fechaParseada) &&
                        fechaParseada.Date == dia
                    )
                );

                ranking.Add(new RankingPacienteDto
                {
                    PacienteId = paciente.Id,
                    Nombre = paciente.Nombre ?? "Sin nombre",
                    DiasConRegistro = diasConRegistro
                });
            }

            return ranking;
        }

        // METODO PARA DEVOLVER ESTADISTICAS DE UN PACIENTE
        public async Task<EstadisticasPacienteDto> CalcularEstadisticasDePaciente(string pacienteId)
        {
            var paciente = await _userRepository.ObtenerUsuarioPorIdAsync(pacienteId);

            var comidasMasRegistradas = CalcularComidasMasRegistradasPorPaciente(paciente);
            var cumplimientoCaloricoDiario = await CalcularCumplimientoCaloricoDiarioDePaciente(paciente);
            var rankingDias = CalcularRankingDeDiasConMasComidas(paciente);

            var estadisticas = new EstadisticasPacienteDto
            {
                ComidasMasPopulares = comidasMasRegistradas,
                CumplimientoCaloricoDiario = cumplimientoCaloricoDiario,
                DiasConMasComidas = rankingDias
            };

            return estadisticas;
        }

        // METODOS PARA ESTADISTICAS DE PACIENTE
        private List<ComidaPopularDto> CalcularComidasMasRegistradasPorPaciente(User paciente)
        {
            var fechaLimite = DateTime.UtcNow.Date.AddDays(-6);

            var comidas = paciente.Paciente?.ComidasRegistradas?
                .Where(c =>
                    !string.IsNullOrEmpty(c.Fecha) &&
                    DateTime.TryParse(c.Fecha, out var fechaParseada) &&
                    fechaParseada.Date >= fechaLimite &&
                    !string.IsNullOrEmpty(c.Nombre))
                .ToList() ?? new List<ComidaRegistrada>();

            var topComidas = comidas
                .GroupBy(c => c.Nombre!.Trim().ToLower())
                .Select(g => new ComidaPopularDto
                {
                    Nombre = g.First().Nombre,
                    Cantidad = g.Count()
                })
                .OrderByDescending(c => c.Cantidad)
                .Take(5)
                .ToList();

            return topComidas;
        }

        private async Task<List<CumplimientoDiarioDto>> CalcularCumplimientoCaloricoDiarioDePaciente(User paciente)
        {
            var dias = Enumerable.Range(0, 7)
                .Select(offset => DateTime.UtcNow.Date.AddDays(-offset))
                .OrderBy(d => d)
                .ToList();

            var resultado = new List<CumplimientoDiarioDto>();

            foreach (var dia in dias)
            {
                double cumplimientoDia = 0;

                var caloriasRecomendadas = paciente.Paciente?.Calorias ?? 0;
                if (caloriasRecomendadas != 0 && paciente.Paciente?.ComidasRegistradas != null)
                {
                    var comidasDelDia = paciente.Paciente.ComidasRegistradas
                        .Where(c =>
                            !string.IsNullOrEmpty(c.Fecha) &&
                            DateTime.TryParse(c.Fecha, out var fechaParseada) &&
                            fechaParseada.Date == dia)
                        .ToList();

                    if (comidasDelDia.Any())
                    {
                        var comidaIds = comidasDelDia
                            .Select(c => c.ComidaId)
                            .Where(id => !string.IsNullOrEmpty(id))
                            .Distinct()
                            .ToList();

                        if (comidaIds.Any())
                        {
                            var comidasConCalorias = await _comidaRepository.ObtenerComidasPorIdsAsync(comidaIds);
                            var dictCalorias = comidasConCalorias.ToDictionary(c => c.Id, c => c.Kcal);

                            int kcalConsumidas = comidasDelDia.Sum(c => dictCalorias.GetValueOrDefault(c.ComidaId, 0));
                            cumplimientoDia = (double)kcalConsumidas / caloriasRecomendadas * 100;
                        }
                    }
                }

                resultado.Add(new CumplimientoDiarioDto
                {
                    Fecha = dia,
                    PorcentajeCumplido = cumplimientoDia
                });
            }

            return resultado;
        }

        private List<RankingDiasDto> CalcularRankingDeDiasConMasComidas(User paciente)
        {
            var culture = new System.Globalization.CultureInfo("es-ES");

            var dias = Enumerable.Range(0, 7)
                .Select(offset => DateTime.UtcNow.Date.AddDays(-offset))
                .ToList();

            var ranking = new List<RankingDiasDto>();

            if (paciente.Paciente?.ComidasRegistradas == null)
            {
                return dias.Select(d => new RankingDiasDto
                {
                    Fecha = $"{culture.DateTimeFormat.GetDayName(d.DayOfWeek)} {d:dd-MM-yyyy}",
                    ComidasRegistradas = 0
                }).ToList();
            }

            var comidasPorDia = dias
                .Select(dia => new
                {
                    Fecha = dia,
                    NombreDia = culture.DateTimeFormat.GetDayName(dia.DayOfWeek),
                    Comidas = paciente.Paciente.ComidasRegistradas
                        .Count(c => !string.IsNullOrEmpty(c.Fecha) &&
                                   DateTime.TryParse(c.Fecha, out var fechaParseada) &&
                                   fechaParseada.Date == dia)
                })
                .OrderByDescending(x => x.Comidas)
                .ThenBy(x => x.Fecha)
                .ToList();

            foreach (var dia in comidasPorDia)
            {
                ranking.Add(new RankingDiasDto
                {
                    Fecha = $"{dia.NombreDia} {dia.Fecha:dd-MM-yyyy}",
                    ComidasRegistradas = dia.Comidas
                });
            }

            return ranking;
        }

        public async Task<List<PlanSemanalDto>> TomarPlanSemanalPorIdAsync(string pacienteId)
        {
            var paciente = await _userRepository.ObtenerUsuarioPorIdAsync(pacienteId);

            if (paciente == null || paciente.Paciente == null)
                throw new Exception("Paciente no encontrado");

            var planSemanal = new List<PlanSemanalDto>();

            foreach (var plan in paciente.Paciente.PlanSemanal)
            {
                var comidas = await _comidaRepository.ObtenerComidasPorIdsAsync(plan.Comidas);

                var comidasDto = comidas.Select(comida => new ComidaPlanSemanalDto
                {
                    Name = comida.Nombre,
                    Kcal = comida.Kcal
                }).ToList();

                planSemanal.Add(new PlanSemanalDto
                {
                    Dia = plan.Dia ?? string.Empty,
                    Comidas = comidasDto
                });
            }

            return planSemanal;
        }

        public async Task<bool> AgregarPlanSemanalAsync(string pacienteId, PlanSemanal plan)
        {
            var usuario = await _userRepository.ObtenerUsuarioPorIdAsync(pacienteId);

            if (usuario == null || usuario.Paciente == null)
                throw new Exception("Paciente no encontrado");

            // Agrega comidas a la lista si no existen
            foreach (var comidaId in plan.Comidas)
            {
                if (!usuario.Paciente.Comidas.Contains(comidaId))
                {
                    usuario.Paciente.Comidas.Add(comidaId);
                }    
            }
            
            // Actualiza el plan semanal
            var planSemanal = usuario.Paciente.PlanSemanal;
            var planExistente = planSemanal.FirstOrDefault(p => p.Dia?.Equals(plan.Dia, StringComparison.OrdinalIgnoreCase) == true);

            if (planExistente != null)
            {
                foreach (var comidaId in plan.Comidas)
                {
                    planExistente.Comidas.Add(comidaId);
                }
            }
            else
            {
                planSemanal.Add(new PlanSemanal
                {
                    Dia = plan.Dia,
                    Comidas = plan.Comidas.Distinct().ToList()
                });
            }

            await _userRepository.EditarUsuarioAsync(usuario);

            return true;
        }
    }
}
