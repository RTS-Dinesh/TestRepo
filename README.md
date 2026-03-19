# MongoDB CRUD Operations with C#

A complete C# application demonstrating Create, Read, Update, and Delete (CRUD) operations using MongoDB as the backend database.

## Features

- ✅ **Create** - Insert new documents into MongoDB
- ✅ **Read** - Retrieve documents by ID, email, or get all documents
- ✅ **Update** - Full document update or partial field updates
- ✅ **Delete** - Delete documents by ID or delete all documents

## Prerequisites

1. **.NET SDK 8.0 or later**
   - Download from [.NET Downloads](https://dotnet.microsoft.com/download)

2. **MongoDB**
   - Install MongoDB locally or use MongoDB Atlas (cloud)
   - For local installation: [MongoDB Community Server](https://www.mongodb.com/try/download/community)
   - For cloud: [MongoDB Atlas](https://www.mongodb.com/cloud/atlas) (free tier available)

## Project Structure

```
.
├── Models/
│   └── User.cs              # User data model
├── Services/
│   └── MongoDbService.cs    # MongoDB CRUD operations service
├── Program.cs               # Main program (hardcoded connection)
├── ProgramWithConfig.cs     # Alternative program using appsettings.json
├── appsettings.json         # Configuration file
├── MongoDbCrud.csproj       # Project file with dependencies
└── README.md               # This file
```

## Setup Instructions

### 1. Install Dependencies

```bash
# Restore NuGet packages
dotnet restore
```

### 2. Configure MongoDB Connection

#### Option A: Update Program.cs directly
Edit `Program.cs` and modify the connection string:
```csharp
string connectionString = "mongodb://localhost:27017"; // or your MongoDB Atlas connection string
string databaseName = "CrudDb";
string collectionName = "Users";
```

#### Option B: Use appsettings.json (Recommended)
Edit `appsettings.json`:
```json
{
  "MongoDb": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "CrudDb",
    "CollectionName": "Users"
  }
}
```

For MongoDB Atlas, use a connection string like:
```
mongodb+srv://username:password@cluster.mongodb.net/?retryWrites=true&w=majority
```

### 3. Run the Application

```bash
# Build the project
dotnet build

# Run the application
dotnet run
```

## Usage Examples

### Create (Insert)

```csharp
var user = new User
{
    Name = "John Doe",
    Email = "john.doe@example.com",
    Age = 30
};

var createdUser = await mongoDbService.CreateUserAsync(user);
```

### Read (Query)

```csharp
// Get all users
var allUsers = await mongoDbService.GetAllUsersAsync();

// Get user by ID
var user = await mongoDbService.GetUserByIdAsync(userId);

// Get user by email
var user = await mongoDbService.GetUserByEmailAsync("john.doe@example.com");
```

### Update

```csharp
// Full update
user.Name = "John Updated";
user.Age = 31;
await mongoDbService.UpdateUserAsync(userId, user);

// Partial update (update specific fields only)
await mongoDbService.UpdateUserFieldsAsync(
    userId, 
    name: "John Updated",
    age: 31
);
```

### Delete

```csharp
// Delete by ID
await mongoDbService.DeleteUserAsync(userId);

// Delete all users (use with caution!)
await mongoDbService.DeleteAllUsersAsync();
```

## MongoDB Service Methods

The `MongoDbService` class provides the following methods:

| Method | Description |
|--------|-------------|
| `CreateUserAsync(User)` | Insert a new user document |
| `GetAllUsersAsync()` | Retrieve all user documents |
| `GetUserByIdAsync(string)` | Retrieve a user by ObjectId |
| `GetUserByEmailAsync(string)` | Retrieve a user by email |
| `UpdateUserAsync(string, User)` | Replace entire user document |
| `UpdateUserFieldsAsync(string, ...)` | Update specific fields only |
| `DeleteUserAsync(string)` | Delete a user by ID |
| `DeleteAllUsersAsync()` | Delete all users (use with caution) |
| `UserExistsAsync(string)` | Check if a user exists |

## Data Model

The `User` model includes:
- `Id` - MongoDB ObjectId (auto-generated)
- `Name` - User's full name
- `Email` - User's email address
- `Age` - User's age
- `CreatedAt` - Timestamp of creation (auto-set to UTC)

## Customization

### Change the Data Model

To use a different model (e.g., Product, Order, etc.):

1. Create a new model class in `Models/` folder
2. Update `MongoDbService` to use your new model:
   ```csharp
   private readonly IMongoCollection<YourModel> _collection;
   ```
3. Update all method signatures to use your model type

### Add More CRUD Operations

Extend `MongoDbService` with additional methods:
- Filter by multiple criteria
- Pagination support
- Sorting
- Aggregation queries

## Troubleshooting

### Connection Issues

- **Error: Unable to connect to MongoDB**
  - Ensure MongoDB is running: `mongod` or check MongoDB Atlas connection
  - Verify connection string is correct
  - Check firewall settings if using remote MongoDB

### Build Errors

- **Error: Package restore failed**
  - Run `dotnet restore` to restore NuGet packages
  - Check internet connection for package downloads

### Runtime Errors

- **Error: Collection not found**
  - MongoDB will create collections automatically on first insert
  - Ensure database name is correct

## Dependencies

- **MongoDB.Driver** (v2.28.0) - Official MongoDB C# driver
- **Microsoft.Extensions.Configuration** (v8.0.0) - Configuration support
- **Microsoft.Extensions.Configuration.Json** (v8.0.0) - JSON configuration support

## License

This project is provided as-is for educational and demonstration purposes.

## Additional Resources

- [MongoDB C# Driver Documentation](https://www.mongodb.com/docs/drivers/csharp/)
- [MongoDB .NET Tutorial](https://www.mongodb.com/docs/drivers/csharp/current/quick-start/)
- [.NET Documentation](https://docs.microsoft.com/dotnet/)
