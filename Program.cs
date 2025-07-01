using NutriCheck.Backend;
using NutriCheck.Backend.Repositories; // 👈 Asegúrate de tener este using
using NutriCheck.Backend.Services;
using QuestPDF.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsProduction())
{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}
// Configurar licencia de QuestPDF
QuestPDF.Settings.License = LicenseType.Community; // 👈 Esta línea es la clave

// Add services to the container.
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseInMemoryDatabase("NutriCheckDb")); // Base temporal para pruebas
builder.Services.AddControllers();
builder.Services.AddSingleton<MongoDBConnection>();
builder.Services.AddEndpointsApiExplorer();

string? value = builder.Configuration.GetSection("Token").Value;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(value)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IComidaRepository, ComidaRepository>();
builder.Services.AddScoped<IComidaService, ComidaService>();

// 👇 Configuración para leer los comentarios XML
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirVarios", policy =>
    {
        policy.SetIsOriginAllowed(origin =>
            origin == "http://localhost:3000" ||
            origin == "https://nutricheck-front.vercel.app")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("PermitirVarios");
app.UseAuthorization();

app.MapControllers();

app.Run();
