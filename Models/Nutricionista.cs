using MongoDB.Bson.Serialization.Attributes;

namespace NutriCheck.Models
{
    public class Nutricionista
    {
        [BsonElement("pacientes")]
        public List<string> Pacientes { get; set; } = new List<string>();
    }
}
