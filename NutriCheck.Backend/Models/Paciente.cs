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

        [BsonElement("nutricionistaId")]
        public string? NutricionistaId { get; set; }
    }
}
