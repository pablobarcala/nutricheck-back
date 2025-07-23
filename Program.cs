using NutriCheck.Backend;
using NutriCheck.Backend.Repositories;
using NutriCheck.Backend.Services;
using QuestPDF.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Configurar URL del servidor
if (builder.Environment.IsProduction())
{
    var port = Environment.GetEnvironmentVariable("PORT")??"8080";
    var url = $"http://0.0.0.0:{port}".Trim(); // 👈 evita espacios
    Console.WriteLine($"🚀 Iniciando en: {url}"); // 👈 debug
    builder.WebHost.UseUrls(url);
}

// Configurar licencia de QuestPDF
QuestPDF.Settings.License = LicenseType.Community;

// Servicios
builder.Services.AddControllers();
builder.Services.AddSingleton<MongoDBConnection>();
builder.Services.AddEndpointsApiExplorer();

string? value = builder.Configuration.GetSection("Token").Value;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
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

// Swagger XML docs
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirVarios", policy =>
    {
        policy.SetIsOriginAllowed(origin =>
            origin == "http://localhost:3000" ||
            origin == "https://nutricheck-front.vercel.app" ||
            origin == "https://www.nutricheck.me" ||
            origin == "https://nutricheck.me")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("PermitirVarios");
app.UseAuthentication(); // 👈 Importante si usás JWT
app.UseAuthorization();

app.MapControllers();
app.Run();
