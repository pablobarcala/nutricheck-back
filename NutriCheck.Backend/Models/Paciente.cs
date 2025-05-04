using MongoDB.Bson.Serialization.Attributes;

namespace NutriCheck.Models
{
    public class Paciente
    {
        [BsonElement("edad")]
        public int Edad { get; set; }

        [BsonElement("genero")]
        public string? Genero { get; set; }

        [BsonElement("altura")]
        public float Altura { get; set; }

        [BsonElement("peso")]
        public float Peso { get; set; }

        [BsonElement("objetivo")]
        public string? Objetivo { get; set; }

        [BsonElement("nutricionistaId")]
        public string NutricionistaId { get; set; }
    }
}
