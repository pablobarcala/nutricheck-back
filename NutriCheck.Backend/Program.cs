using NutriCheck.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using NutriCheck.Models;
using QuestPDF.Infrastructure;
using NutriCheck.Backend; // 👈 Asegúrate de tener este using

var builder = WebApplication.CreateBuilder(args);

// Configurar licencia de QuestPDF
QuestPDF.Settings.License = LicenseType.Community; // 👈 Esta línea es la clave

// Add services to the container.
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseInMemoryDatabase("NutriCheckDb")); // Base temporal para pruebas
builder.Services.AddControllers();
builder.Services.AddSingleton<MongoDBConnection>();
builder.Services.AddEndpointsApiExplorer();

// 👇 Configuración para leer los comentarios XML
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// Precarga de nutricionista de prueba
//using (var scope = app.Services.CreateScope())
//{
//    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

//    // Agregar nutricionista solo si no existe
//    if (!db.Nutricionistas.Any())
//    {
//        db.Nutricionistas.Add(new Nutricionista
//        {
//            Nombre = "Martin Sanchez",
//            Email = "martin@nutricheck.com",
//            Password = "1234" // Por ahora sin encriptar
//        });

//        db.SaveChanges();
//    }
//}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
