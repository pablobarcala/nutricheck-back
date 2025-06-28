using NutriCheck.Backend.Models;

namespace NutriCheck.Backend.Dtos
{
    public class EstadisticasGlobalesDto
    {
        public double PromedioCumplimientoCalorico { get; set; }
        public int PacientesConBajoCumplimiento { get; set; }
        public List<CumplimientoDiarioDto> CumplimientoCaloricoPorDia { get; set; }
        public List<ComidaPopularDto> ComidasMasPopulares { get; set; }
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
}
