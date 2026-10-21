using DA;
using Flujo;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// OpenAPI
builder.Services.AddOpenApi();

// Registro de dependencias de la capa de acceso a datos (DA)
builder.Services.AgregarCapaDatos(builder.Configuration);

// Registro de dependencias de la capa de lógica de negocio (Flujo)
builder.Services.AgregarCapaFlujo(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
