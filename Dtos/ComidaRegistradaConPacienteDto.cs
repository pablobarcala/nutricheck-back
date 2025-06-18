using NutriCheck.Models;

namespace NutriCheck.Backend.Dtos
{
    public class ComidaRegistradaConPacienteDto
    {
        public List<ComidaParaFront> Comidas { get; set; } = new List<ComidaParaFront>();
        public string Nombre { get; set; }
    }

    public class ComidaParaFront
    {
        public string Title { get; set; }
        public string Horario { get; set; }
        public string Fecha { get; set; }
    }
}
