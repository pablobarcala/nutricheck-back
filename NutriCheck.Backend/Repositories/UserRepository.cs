using MongoDB.Driver;
using NutriCheck.Backend.Dtos;
using NutriCheck.Backend.Models;

namespace NutriCheck.Backend.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;
        public UserRepository(MongoDBConnection connection)
        {
            _users = connection.GetCollection<User>("users");
        }

        // Método para guardar usuario en la base de datos
        public async Task<bool> GuardarUsuarioAsync(User user)
        {
            await _users.InsertOneAsync(user);
            return true;
        }

        // Método para verificar si el usuario existe por email
        public async Task<bool> UsuarioExisteConMailAsync(string email)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Email, email);
            var user = await _users.Find(filter).FirstOrDefaultAsync();
            return user != null;
        }

        // Método para obtener usuario por email
        public async Task<User?> ObtenerUsuarioPorEmailAsync(string email)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Email, email);
            var user = await _users.Find(filter).FirstOrDefaultAsync();
            return user;
        }
    }
}
