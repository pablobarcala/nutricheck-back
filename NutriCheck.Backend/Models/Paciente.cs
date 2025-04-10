namespace NutriCheck.Models
{
    public class Paciente
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Edad { get; set; }
        public string Genero { get; set; }
        public float Altura { get; set; }
        public float Peso { get; set; }
        public string Objetivo { get; set; }
        public int NutricionistaId { get; set; }
    }
}
