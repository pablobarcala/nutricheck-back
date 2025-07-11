using MongoDB.Bson.Serialization.Attributes;

namespace NutriCheck.Models
{
    public class Paciente
    {
        [BsonElement("peso")]
        public float Peso { get; set; }

        [BsonElement("altura")]
        public float Altura { get; set; }

        [BsonElement("fechaNacimiento")]
        public string FechaNacimiento { get; set; }

        [BsonElement("sexo")]
        public string? Sexo { get; set; }
        
        [BsonElement("actividad")]
        public string? Actividad { get; set; }

        [BsonElement("calorias")]
        public int Calorias { get; set; }

        [BsonElement("objetivo")]
        public string? Objetivo { get; set; }

        [BsonElement("grasas")]
        public double Grasas { get; set; } = 0;

        [BsonElement("carbohidratos")]
        public double Carbohidratos { get; set; } = 0;

        [BsonElement("proteinas")]
        public double Proteinas { get; set; } = 0;

        [BsonElement("nutricionistaId")]
        public string? NutricionistaId { get; set; }

        [BsonElement("comidas")]
        public List<string> Comidas { get; set; } = new List<string>();

        [BsonElement("comidasRegistradas")]
        public List<ComidaRegistrada> ComidasRegistradas { get; set; } = new List<ComidaRegistrada>();
    }

    public class ComidaRegistrada
    {
        [BsonElement("comidaId")]
        public string? ComidaId { get; set; }

        [BsonElement("nombre")]
        public string? Nombre { get; set; }

        [BsonElement("horario")]
        public string? Horario { get; set; }

        [BsonElement("fecha")]
        public string? Fecha { get; set; }

        [BsonElement("kcal")]
        public int? Kcal { get; set; }
    }
}
