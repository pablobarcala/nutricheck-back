using NutriCheck.Backend.Models;

namespace NutriCheck.Backend.Dtos
{
    public class EstadisticasGlobalesDto
    {
        public double PromedioCumplimientoCalorico { get; set; }
        public int PacientesConBajoCumplimiento { get; set; }
        public List<CumplimientoDiarioDto> CumplimientoCaloricoPorDia { get; set; } = new();
        public List<ComidaPopularDto> ComidasMasPopulares { get; set; } = new();
        public List<NivelActividadDto> PacientesPorNivelActividad { get; set; } = new();
        public List<RankingPacienteDto> PacientesMasConstantes { get; set; } = new();
        public List<RankingPacienteDto> PacientesConMasFaltantes { get; set; } = new();
    }

    public class CumplimientoDiarioDto
    {
        public DateTime Fecha { get; set; }
        public double PorcentajeCumplido { get; set; }
    }

    public class ComidaPopularDto
    {
        public string Nombre { get; set; } = string.Empty;
        public int Cantidad { get; set; } // Cantidad de veces registrada en la semana
    }

    public class NivelActividadDto
    {
        public string Nivel { get; set; } = string.Empty;
        public int Cantidad { get; set; }
    }

    public class RankingPacienteDto
    {
        public string PacienteId { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public int DiasConRegistro { get; set; }
    }
}
