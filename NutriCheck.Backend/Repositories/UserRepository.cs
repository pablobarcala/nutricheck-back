using MongoDB.Driver;
using NutriCheck.Backend.Dtos;
using NutriCheck.Backend.Models;
using NutriCheck.Models;

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

        // Método para obtener usuario por ID
        public async Task<User?> ObtenerUsuarioPorIdAsync(string id)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, id);
            var user = await _users.Find(filter).FirstOrDefaultAsync();
            return user;
        }

        // Método para guardar datos del paciente
        public async Task<bool> GuardarDatosPacienteAsync(string userId, Paciente datosPaciente)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
            var update = Builders<User>.Update.Set(u => u.Paciente, datosPaciente);
            var result = await _users.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        // Método para editar usuario
        public async Task<bool> EditarUsuarioAsync(User user)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
            var update = Builders<User>.Update.Set(u => u, user);
            var result = await _users.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }
    }
}
