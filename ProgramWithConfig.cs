using Microsoft.Extensions.Configuration;
using MongoDbCrud.Models;
using MongoDbCrud.Services;

namespace MongoDbCrud
{
    // Alternative Program class that uses appsettings.json for configuration
    class ProgramWithConfig
    {
        private static MongoDbService? _mongoDbService;

        static async Task Main(string[] args)
        {
            // Load configuration from appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            string connectionString = configuration["MongoDb:ConnectionString"] 
                ?? "mongodb://localhost:27017";
            string databaseName = configuration["MongoDb:DatabaseName"] 
                ?? "CrudDb";
            string collectionName = configuration["MongoDb:CollectionName"] 
                ?? "Users";

            _mongoDbService = new MongoDbService(connectionString, databaseName, collectionName);

            Console.WriteLine("=== MongoDB CRUD Operations Demo (Using Configuration) ===\n");
            Console.WriteLine($"Connection: {connectionString}");
            Console.WriteLine($"Database: {databaseName}");
            Console.WriteLine($"Collection: {collectionName}\n");

            await RunCrudOperations();
        }

        static async Task RunCrudOperations()
        {
            try
            {
                // CREATE - Add new users
                Console.WriteLine("1. Creating users...");
                var user1 = new User
                {
                    Name = "Alice Johnson",
                    Email = "alice.johnson@example.com",
                    Age = 28
                };

                var user2 = new User
                {
                    Name = "Bob Williams",
                    Email = "bob.williams@example.com",
                    Age = 35
                };

                var createdUser1 = await _mongoDbService!.CreateUserAsync(user1);
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

                // UPDATE - Full update
                if (createdUser1.Id != null)
                {
                    Console.WriteLine("4. Updating user (full update)...");
                    createdUser1.Name = "Alice Updated";
                    createdUser1.Age = 29;
                    var updateResult = await _mongoDbService.UpdateUserAsync(createdUser1.Id, createdUser1);
                    Console.WriteLine($"  Update result: {(updateResult ? "Success" : "Failed")}\n");
                }

                // UPDATE - Partial update
                if (createdUser2.Id != null)
                {
                    Console.WriteLine("5. Updating user (partial update)...");
                    var partialUpdateResult = await _mongoDbService.UpdateUserFieldsAsync(
                        createdUser2.Id, 
                        name: "Bob Updated",
                        age: 36
                    );
                    Console.WriteLine($"  Partial update result: {(partialUpdateResult ? "Success" : "Failed")}\n");
                }

                // READ - Verify updates
                Console.WriteLine("6. Verifying updates...");
                var updatedUsers = await _mongoDbService.GetAllUsersAsync();
                foreach (var user in updatedUsers)
                {
                    Console.WriteLine($"  - {user.Name} ({user.Email}), Age: {user.Age}");
                }
                Console.WriteLine();

                // DELETE - Delete a user
                if (createdUser2.Id != null)
                {
                    Console.WriteLine($"7. Deleting user (ID: {createdUser2.Id})...");
                    var deleteResult = await _mongoDbService.DeleteUserAsync(createdUser2.Id);
                    Console.WriteLine($"  Delete result: {(deleteResult ? "Success" : "Failed")}\n");
                }

                // READ - Verify deletion
                Console.WriteLine("8. Verifying deletion...");
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
