namespace NutriCheck.Backend.Dtos
{
    public class PlanSemanalDto
    {
        public string Dia { get; set; } = string.Empty;
        public List<ComidaPlanSemanalDto> Comidas { get; set; } = new List<ComidaPlanSemanalDto>();
    }

    public class ComidaPlanSemanalDto
    {
        public string Name { get; set; } = string.Empty;
        public int Kcal { get; set; }
    }
}