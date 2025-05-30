namespace NutriCheck.Backend.Dtos
{
    public class PacienteBuscadoDto
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string FechaNacimiento { get; set; }
        public float Peso { get; set; }
        public float Altura { get; set; }
        public string Sexo { get; set; }
        public int Calorias { get; set; }
    }
}
