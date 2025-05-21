using MongoDB.Bson.Serialization.Attributes;
using NutriCheck.Models;

namespace NutriCheck.Backend.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("nombre")]
        public string? Nombre { get; set; }

        [BsonElement("email")]
        public string? Email { get; set; }

        [BsonElement("passwordHash")]
        public byte[] PasswordHash { get; set; }

        [BsonElement("passwordSalt")]
        public byte[] PasswordSalt { get; set; }

        [BsonElement("rol")]
        public string Rol { get; set; } = string.Empty; // "nutricionista" o "paciente"

        [BsonElement("paciente")]
        public Paciente? Paciente { get; set; } // Solo si es paciente

        [BsonElement("nutricionista")]
        public Nutricionista? Nutricionista { get; set; } // Solo si es nutricionista
    }
}
