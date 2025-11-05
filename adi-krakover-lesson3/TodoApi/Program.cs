


// using Microsoft.EntityFrameworkCore;
// using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
// using TodoApi; // מנוכח שבדקנו, זה ה-Namespace הנכון עבור Item ו-ToDoDbContext


// var builder = WebApplication.CreateBuilder(args);

// // --- הוספת החיבור למסד הנתונים (שלב 1) ---
// var connectionString = builder.Configuration.GetConnectionString("ToDoDB");
// var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
   
// builder.Services.AddDbContext<ToDoDbContext>(options =>
//     options.UseMySql(
//         connectionString,
//         ServerVersion.AutoDetect(connectionString)
//     )
// );
// // ------------------------------------------

// var app = builder.Build();
// app.UseCors(MyAllowSpecificOrigins);
// builder.Services.AddCors(options =>
// {
//     options.AddPolicy(name: MyAllowSpecificOrigins,
//                       policy =>
//                       {
//                           policy.AllowAnyOrigin() // מאפשר מכל דומיין
//                                 .AllowAnyHeader() // מאפשר את כל כותרות הבקשה
//                                 .AllowAnyMethod(); // מאפשר את כל המתודות (GET, POST, PUT, DELETE)
//                       });
// });

// // 1. שליפת כל המשימות (GET: /api/items)
// app.MapGet("/api/items", async (ToDoDbContext context) =>
// {
//     return await context.Item.ToListAsync();
// });

// // 2. הוספת משימה חדשה (POST: /api/items)
// app.MapPost("/api/items", async (ToDoDbContext context, Item item) =>
// {
//     context.Item.Add(item);
//     await context.SaveChangesAsync();
//     return Results.Created($"/api/items/{item.Id}", item);
// });

// // 3. עדכון משימה (PUT: /api/items/{id})
// app.MapPut("/api/items/{id}", async (ToDoDbContext context, int id, Item updatedItem) =>
// {
//     var itemToUpdate = await context.Item.FindAsync(id);

//     if (itemToUpdate == null) return Results.NotFound();

//     itemToUpdate.Name = updatedItem.Name;
//     itemToUpdate.IsComplete = updatedItem.IsComplete;

//     await context.SaveChangesAsync();
//     return Results.NoContent();
// });

// // 4. מחיקת משימה (DELETE: /api/items/{id})
// app.MapDelete("/api/items/{id}", async (ToDoDbContext context, int id) =>
// {
//     var itemToDelete = await context.Item.FindAsync(id);

//     if (itemToDelete == null) return Results.NotFound();

//     context.Item.Remove(itemToDelete);
//     await context.SaveChangesAsync();
//     return Results.NoContent();
// });
// // ------------------------------------------

// app.Run();


using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using TodoApi; // מנוכח שבדקנו, זה ה-Namespace הנכון עבור Item ו-ToDoDbContext

// הגדרת שם למדיניות CORS
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

// --- הוספת שירותים ל-Swagger/OpenAPI ---
// חובה להוסיף את שירותי יצירת ה-Swagger JSON
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// ------------------------------------------

// --- הוספת שירות CORS (Services) ---
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          // מאפשר לכל Domain לפנות ל-API (הרשאה כללית)
                          policy.AllowAnyOrigin()
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});
// ------------------------------------------

// --- הוספת החיבור למסד הנתונים ---
var connectionString = builder.Configuration.GetConnectionString("ToDoDB");

builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    )
);
// ------------------------------------------

var app = builder.Build();

// --- 2. שימוש ב-Swagger UI ו-JSON ---
// חובה להפעיל את ה-Swagger UI רק בסביבת פיתוח
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    // הפנייה אוטומטית מכתובת הבסיס ל-Swagger UI
    app.MapGet("/", () => Results.Redirect("/swagger"));
}
// ------------------------------------

// --- 2. שימוש ב-CORS (Middleware) ---
// חובה להפעיל לפני MapGet/MapPost כדי לאפשר גישה חיצונית
app.UseCors(MyAllowSpecificOrigins);
// ------------------------------------


// --- ניתובים לניהול משימות (CRUD) ---

// 1. שליפת כל המשימות (GET: /api/items)
app.MapGet("/api/items", async (ToDoDbContext context) =>
{
    return await context.Item.ToListAsync();
});

// 2. הוספת משימה חדשה (POST: /api/items)
app.MapPost("/api/items", async (ToDoDbContext context, Item item) =>
{
    context.Item.Add(item);
    await context.SaveChangesAsync();
    return Results.Created($"/api/items/{item.Id}", item);
});

// 3. עדכון משימה (PUT: /api/items/{id})
app.MapPut("/api/items/{id}", async (ToDoDbContext context, int id, Item updatedItem) =>
{
    var itemToUpdate = await context.Item.FindAsync(id);

    if (itemToUpdate == null) return Results.NotFound();

    itemToUpdate.Name = updatedItem.Name;
    itemToUpdate.IsComplete = updatedItem.IsComplete;

    await context.SaveChangesAsync();
    return Results.NoContent();
});

// 4. מחיקת משימה (DELETE: /api/items/{id})
app.MapDelete("/api/items/{id}", async (ToDoDbContext context, int id) =>
{
    var itemToDelete = await context.Item.FindAsync(id);

    if (itemToDelete == null) return Results.NotFound();

    context.Item.Remove(itemToDelete);
    await context.SaveChangesAsync();
    return Results.NoContent();
});
// ------------------------------------------

app.Run();