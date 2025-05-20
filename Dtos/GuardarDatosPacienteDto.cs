namespace NutriCheck.Backend.Dtos
{
    public class GuardarDatosPacienteDto
    {
        public float Peso { get; set; }
        public float Altura { get; set; }
        public string FechaNacimiento { get; set; } 
        public string Sexo { get; set; }
        public string Actividad { get; set; }
        public int Calorias { get; set; }
    }
}
