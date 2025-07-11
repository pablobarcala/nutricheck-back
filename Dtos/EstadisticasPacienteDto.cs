namespace NutriCheck.Backend.Dtos
{
    public class EstadisticasPacienteDto
    {
        public List<ComidaPopularDto> ComidasMasPopulares { get; set; } = new();
        public List<CumplimientoDiarioDto> CumplimientoCaloricoDiario { get; set; } = new();
        public List<RankingDiasDto> DiasConMasComidas { get; set; } = new();
    }

    public class RankingDiasDto
    {
        public string Fecha { get; set; } = string.Empty;
        public int ComidasRegistradas { get; set; }
    }
}
