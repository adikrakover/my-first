using Microsoft.EntityFrameworkCore;
using TodoApi;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);

// --- Swagger Services ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- CORS Configuration ---
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins(
                                    "http://localhost:3000",
                                    "https://adi-krakovr-client.onrender.com",
                                    "https://todo-client-krakover.onrender.com"
                                )
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

// --- Database Connection ---
// ניסיון לקרוא מ-Environment Variables או מ-appsettings
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
    ?? "Server=bm0udkky9qqadnpzi1xb-mysql.services.clever-cloud.com;Database=bm0udkky9qqadnpzi1xb;Uid=uivttg33myotjsre;Pwd=JEjXkMzg0rtZFK1syhyB;Port=3306";

builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    )
);

var app = builder.Build();

// --- Auto-create database and run migrations on startup ---
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ToDoDbContext>();
    try
    {
        // יוצר את מסד הנתונים והטבלאות אוטומטית
        dbContext.Database.EnsureCreated();
        Console.WriteLine("Database created/verified successfully!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database initialization error: {ex.Message}");
    }
}

// --- Swagger UI (Development and Production) ---
app.UseSwagger();
app.UseSwaggerUI();

// --- CORS Middleware ---
app.UseCors(MyAllowSpecificOrigins);

// --- Root Endpoint ---
app.MapGet("/", () => Results.Ok(new
{
    message = "Todo API is running successfully!",
    version = "1.0",
    endpoints = new
    {
        getAllItems = "/api/items",
        swagger = "/swagger",
        health = "/health"
    }
}));

// --- Health Check Endpoint ---
app.MapGet("/health", async (ToDoDbContext context) =>
{
    try
    {
        await context.Database.CanConnectAsync();
        var itemCount = await context.Item.CountAsync();
        return Results.Ok(new
        {
            status = "healthy",
            database = "connected",
            itemsCount = itemCount
        });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Database connection failed: {ex.Message}");
    }
});

// --- CRUD Endpoints ---

// 1. Get all items
app.MapGet("/api/items", async (ToDoDbContext context) =>
{
    try
    {
        var items = await context.Item.ToListAsync();
        return Results.Ok(items);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error getting items: {ex.Message}");
        return Results.Problem($"Error: {ex.Message}");
    }
});

// 2. Get single item by id
app.MapGet("/api/items/{id}", async (ToDoDbContext context, int id) =>
{
    var item = await context.Item.FindAsync(id);
    return item == null ? Results.NotFound() : Results.Ok(item);
});

// 3. Create new item
app.MapPost("/api/items", async (ToDoDbContext context, Item item) =>
{
    try
    {
        context.Item.Add(item);
        await context.SaveChangesAsync();
        return Results.Created($"/api/items/{item.Id}", item);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error creating item: {ex.Message}");
        return Results.Problem($"Error: {ex.Message}");
    }
});

// 4. Update item
app.MapPut("/api/items/{id}", async (ToDoDbContext context, int id, Item updatedItem) =>
{
    try
    {
        var itemToUpdate = await context.Item.FindAsync(id);
        if (itemToUpdate == null) return Results.NotFound();

        itemToUpdate.Name = updatedItem.Name;
        itemToUpdate.IsComplete = updatedItem.IsComplete;

        await context.SaveChangesAsync();
        return Results.NoContent();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error updating item: {ex.Message}");
        return Results.Problem($"Error: {ex.Message}");
    }
});

// 5. Delete item
app.MapDelete("/api/items/{id}", async (ToDoDbContext context, int id) =>
{
    try
    {
        var itemToDelete = await context.Item.FindAsync(id);
        if (itemToDelete == null) return Results.NotFound();

        context.Item.Remove(itemToDelete);
        await context.SaveChangesAsync();
        return Results.NoContent();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error deleting item: {ex.Message}");
        return Results.Problem($"Error: {ex.Message}");
    }
});

app.Run();