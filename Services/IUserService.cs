﻿using NutriCheck.Backend.Dtos;
using NutriCheck.Backend.Models;
using NutriCheck.Models;

namespace NutriCheck.Backend.Services
{
    public interface IUserService
    {
        Task<bool> RegistrarUsuarioAsync(RegistroUserDto user);
        Task<string> LoginUsuarioAsync(LoginUserDto user);
        Task<User> ObtenerUsuarioPorIdAsync(string userId);
        Task<bool> GuardarDatosPaciente(string userId, GuardarDatosPacienteDto datosPaciente);
        Task<bool> AgregarPacienteEnNutricionistaAsync(string nutricionistaId, string pacienteId);
        Task<List<PacienteBuscadoDto>?> BuscarPacientesPorNombre(string nombre);
        Task<List<Comida>> ObtenerComidasDelPaciente(string pacienteId);
        Task<bool> AgregarComidaAPaciente(string pacienteId, string comidaId);
        Task<ValoresNutricionalesDto> ObtenerValoresNutricionalesDelPaciente(string userId);
        Task<bool> EditarValoresNutricionales(string userId, ValoresNutricionalesDto valoresNutricionales);
        Task<List<User>> ObtenerPacientesDelNutricionista(string nutricionistaId);
        Task<List<ComidaRegistrada>> ObtenerComidasRegistradasDePaciente(string userId);
        Task<List<ComidaRegistradaConPacienteDto>> ObtenerComidasRegistradasConInfoPaciente(string nutricionistaId);
        Task<bool> RegistrarComidaEnPaciente(string userId, ComidaRegistrada comida);
        Task<EstadisticasGlobalesDto> CalcularEstadisticasDeNutricionista(string nutricionistaId);
        Task<EstadisticasPacienteDto> CalcularEstadisticasDePaciente(string pacienteId);
        Task<List<PlanSemanalDto>> TomarPlanSemanalPorIdAsync(string pacienteId);
        Task<bool> AgregarPlanSemanalAsync(string pacienteId, PlanSemanal plan);
    }
}
