using MongoDB.Driver;
using MongoDbCrud.Models;

namespace MongoDbCrud.Services
{
    public class MongoDbService
    {
        private readonly IMongoCollection<User> _users;

        public MongoDbService(string connectionString, string databaseName, string collectionName)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _users = database.GetCollection<User>(collectionName);
        }

        // CREATE - Insert a new user
        public async Task<User> CreateUserAsync(User user)
        {
            await _users.InsertOneAsync(user);
            return user;
        }

        // READ - Get all users
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _users.Find(user => true).ToListAsync();
        }

        // READ - Get user by ID
        public async Task<User?> GetUserByIdAsync(string id)
        {
            return await _users.Find(user => user.Id == id).FirstOrDefaultAsync();
        }

        // READ - Get users by email
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _users.Find(user => user.Email == email).FirstOrDefaultAsync();
        }

        // UPDATE - Update user by ID
        public async Task<bool> UpdateUserAsync(string id, User updatedUser)
        {
            var result = await _users.ReplaceOneAsync(user => user.Id == id, updatedUser);
            return result.ModifiedCount > 0;
        }

        // UPDATE - Partial update (update specific fields)
        public async Task<bool> UpdateUserFieldsAsync(string id, string? name = null, string? email = null, int? age = null)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, id);
            var update = Builders<User>.Update;

            var updateDefinitions = new List<UpdateDefinition<User>>();

            if (!string.IsNullOrEmpty(name))
                updateDefinitions.Add(update.Set(u => u.Name, name));

            if (!string.IsNullOrEmpty(email))
                updateDefinitions.Add(update.Set(u => u.Email, email));

            if (age.HasValue)
                updateDefinitions.Add(update.Set(u => u.Age, age.Value));

            if (updateDefinitions.Count == 0)
                return false;

            var combinedUpdate = update.Combine(updateDefinitions);
            var result = await _users.UpdateOneAsync(filter, combinedUpdate);
            return result.ModifiedCount > 0;
        }

        // DELETE - Delete user by ID
        public async Task<bool> DeleteUserAsync(string id)
        {
            var result = await _users.DeleteOneAsync(user => user.Id == id);
            return result.DeletedCount > 0;
        }

        // DELETE - Delete all users (use with caution!)
        public async Task<long> DeleteAllUsersAsync()
        {
            var result = await _users.DeleteManyAsync(user => true);
            return result.DeletedCount;
        }

        // Additional helper method - Check if user exists
        public async Task<bool> UserExistsAsync(string id)
        {
            var user = await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
            return user != null;
        }
    }
}
