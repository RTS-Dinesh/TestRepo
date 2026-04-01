using MongoDbCrud.Models;
using MongoDbCrud.Services;

namespace MongoDbCrud
{
    class Program
    {
        private static MongoDbService? _mongoDbService;

        static async Task Main(string[] args)
        {
            // MongoDB connection string - Update this with your MongoDB connection string
            // Format: mongodb://localhost:27017 or mongodb://username:password@host:port
            string connectionString = "mongodb://localhost:27017";
            string databaseName = "CrudDb";
            string collectionName = "Users";

            _mongoDbService = new MongoDbService(connectionString, databaseName, collectionName);

            Console.WriteLine("=== MongoDB CRUD Operations Demo ===\n");

            try
            {
                // CREATE - Add new users
                Console.WriteLine("1. Creating users...");
                var user1 = new User
                {
                    Name = "John Doe",
                    Email = "john.doe@example.com",
                    Age = 30
                };

                var user2 = new User
                {
                    Name = "Jane Smith",
                    Email = "jane.smith@example.com",
                    Age = 25
                };

                var createdUser1 = await _mongoDbService.CreateUserAsync(user1);
                var createdUser2 = await _mongoDbService.CreateUserAsync(user2);

                Console.WriteLine($"Created User 1: {createdUser1.Name} (ID: {createdUser1.Id})");
                Console.WriteLine($"Created User 2: {createdUser2.Name} (ID: {createdUser2.Id})\n");

                // READ - Get all users
                Console.WriteLine("2. Reading all users...");
                var allUsers = await _mongoDbService.GetAllUsersAsync();
                foreach (var user in allUsers)
                {
                    Console.WriteLine($"  - {user.Name} ({user.Email}), Age: {user.Age}, ID: {user.Id}");
                }
                Console.WriteLine();

                // READ - Get user by ID
                if (createdUser1.Id != null)
                {
                    Console.WriteLine($"3. Reading user by ID ({createdUser1.Id})...");
                    var userById = await _mongoDbService.GetUserByIdAsync(createdUser1.Id);
                    if (userById != null)
                    {
                        Console.WriteLine($"  Found: {userById.Name} ({userById.Email})\n");
                    }
                }

                // READ - Get user by email
                Console.WriteLine("4. Reading user by email (jane.smith@example.com)...");
                var userByEmail = await _mongoDbService.GetUserByEmailAsync("jane.smith@example.com");
                if (userByEmail != null)
                {
                    Console.WriteLine($"  Found: {userByEmail.Name}, Age: {userByEmail.Age}\n");
                }

                // UPDATE - Full update
                if (createdUser1.Id != null)
                {
                    Console.WriteLine("5. Updating user (full update)...");
                    createdUser1.Name = "John Updated";
                    createdUser1.Age = 31;
                    var updateResult = await _mongoDbService.UpdateUserAsync(createdUser1.Id, createdUser1);
                    Console.WriteLine($"  Update result: {(updateResult ? "Success" : "Failed")}\n");
                }

                // UPDATE - Partial update
                if (createdUser2.Id != null)
                {
                    Console.WriteLine("6. Updating user (partial update - age only)...");
                    var partialUpdateResult = await _mongoDbService.UpdateUserFieldsAsync(
                        createdUser2.Id, 
                        age: 26
                    );
                    Console.WriteLine($"  Partial update result: {(partialUpdateResult ? "Success" : "Failed")}\n");
                }

                // READ - Verify updates
                Console.WriteLine("7. Verifying updates...");
                var updatedUsers = await _mongoDbService.GetAllUsersAsync();
                foreach (var user in updatedUsers)
                {
                    Console.WriteLine($"  - {user.Name} ({user.Email}), Age: {user.Age}");
                }
                Console.WriteLine();

                // DELETE - Delete a user
                if (createdUser2.Id != null)
                {
                    Console.WriteLine($"8. Deleting user (ID: {createdUser2.Id})...");
                    var deleteResult = await _mongoDbService.DeleteUserAsync(createdUser2.Id);
                    Console.WriteLine($"  Delete result: {(deleteResult ? "Success" : "Failed")}\n");
                }

                // READ - Verify deletion
                Console.WriteLine("9. Verifying deletion...");
                var remainingUsers = await _mongoDbService.GetAllUsersAsync();
                Console.WriteLine($"  Remaining users: {remainingUsers.Count}");
                foreach (var user in remainingUsers)
                {
                    Console.WriteLine($"  - {user.Name} ({user.Email})");
                }
                Console.WriteLine();

                Console.WriteLine("=== CRUD Operations Demo Completed ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
        }
    }
}
