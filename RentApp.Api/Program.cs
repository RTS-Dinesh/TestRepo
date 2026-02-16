using Microsoft.EntityFrameworkCore;
using RentApp.Api.Data;
using RentApp.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// DbContext: use InMemory when "UseInMemory" is true, otherwise SQL Server
var useInMemory = builder.Configuration.GetValue<string>("ConnectionStrings:UseInMemory")?.Equals("true", StringComparison.OrdinalIgnoreCase) == true;
if (useInMemory)
{
    builder.Services.AddDbContext<RentAppDbContext>(options =>
        options.UseInMemoryDatabase("RentAppDb"));
}
else
{
    builder.Services.AddDbContext<RentAppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

// Seed categories when using InMemory
if (useInMemory)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<RentAppDbContext>();
    if (!await db.Categories.AnyAsync())
    {
        var categories = new[]
        {
            new Category { Name = "Electronics", Description = "Phones, laptops, cameras, gadgets", IconName = "devices" },
            new Category { Name = "Vehicles", Description = "Cars, bikes, scooters", IconName = "directions_car" },
            new Category { Name = "Property", Description = "Apartments, rooms, offices", IconName = "home" },
            new Category { Name = "Tools & Equipment", Description = "Power tools, gardening, construction", IconName = "build" },
            new Category { Name = "Clothing & Accessories", Description = "Formal wear, costumes, accessories", IconName = "checkroom" },
            new Category { Name = "Sports & Outdoors", Description = "Bikes, camping gear, sports equipment", IconName = "sports" },
            new Category { Name = "Events & Party", Description = "Decorations, furniture, sound systems", IconName = "celebration" },
            new Category { Name = "Books & Media", Description = "Books, games, movies", IconName = "menu_book" },
            new Category { Name = "Other", Description = "Everything else", IconName = "category" }
        };
        db.Categories.AddRange(categories);
        await db.SaveChangesAsync();
    }
}

app.Run();
