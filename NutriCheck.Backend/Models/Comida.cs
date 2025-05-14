using MongoDB.Bson.Serialization.Attributes;

namespace NutriCheck.Models
{
    public class Comida
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("nutricionistaId")]
        public string? NutricionistaId { get; set; }

        [BsonElement("tipo")]
        public string? Tipo { get; set; } // Desayuno, Almuerzo, Merienda, Cena

        [BsonElement("nombre")]
        public string? Nombre { get; set; } // Ej: "Tostadas con queso"

        [BsonElement("hidratos")]
        public int Hidratos { get; set; } // En gramos

        [BsonElement("proteinas")]
        public int Proteinas { get; set; } // En gramos

        [BsonElement("grasas")]
        public int Grasas { get; set; } // En gramos

        [BsonElement("kcal")]
        public int Kcal { get; set; }
    }
}
