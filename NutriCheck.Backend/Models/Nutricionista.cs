using MongoDB.Bson.Serialization.Attributes;

namespace NutriCheck.Models
{
    public class Nutricionista
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("nombre")]
        public string? Nombre { get; set; }

        [BsonElement("email")]
        public string? Email { get; set; }

        [BsonElement("password")]
        public string? Password { get; set; } // Simple por ahora

        [BsonElement("pacientes")]
        public List<string> Pacientes { get; set; } = new List<string>();
    }
}
